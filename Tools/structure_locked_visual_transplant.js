const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

const projectRoot = path.resolve(__dirname, '..');
const rollbackRoot = path.join(projectRoot, 'StructureLocked_Rollback_20260821', 'Assets');
const assetsRoot = path.join(projectRoot, 'Assets');
const reportPath = path.join(projectRoot, 'StructureLocked_Restore_Report_20260821.md');

function parseDocuments(text) {
  const matches = [...text.matchAll(/^--- !u!(\d+) &(-?\d+)[^\r\n]*(?:\r?\n|$)/gm)];
  const prefix = matches.length ? text.slice(0, matches[0].index) : text;
  const docs = [];
  for (let i = 0; i < matches.length; i += 1) {
    const match = matches[i];
    const start = match.index;
    const bodyStart = start + match[0].length;
    const end = i + 1 < matches.length ? matches[i + 1].index : text.length;
    docs.push({
      classId: match[1],
      fileId: match[2],
      header: match[0],
      body: text.slice(bodyStart, end),
    });
  }
  return { prefix, docs };
}

function field(body, name) {
  const escaped = name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const match = body.match(new RegExp(`^\\s*${escaped}:\\s*(.*)$`, 'm'));
  return match ? match[1].trim() : null;
}

function fileIdReference(body, name) {
  const value = field(body, name);
  if (!value) return null;
  const match = value.match(/\{fileID:\s*(-?\d+)/);
  return match ? match[1] : null;
}

function makeIndex(parsed) {
  const gameObjects = new Map();
  const transforms = new Map();

  for (const doc of parsed.docs) {
    if (doc.classId === '1') {
      gameObjects.set(doc.fileId, { name: field(doc.body, 'm_Name') || `GameObject_${doc.fileId}` });
    } else if (doc.classId === '4' || doc.classId === '224') {
      const childrenBlock = doc.body.match(/^\s*m_Children:\s*\r?\n((?:\s*-\s*\{fileID:\s*-?\d+\}\s*\r?\n)*)/m);
      const children = childrenBlock
        ? [...childrenBlock[1].matchAll(/\{fileID:\s*(-?\d+)\}/g)].map((match) => match[1])
        : [];
      transforms.set(doc.fileId, {
        gameObject: fileIdReference(doc.body, 'm_GameObject'),
        father: fileIdReference(doc.body, 'm_Father') || '0',
        children,
      });
    }
  }

  const segmentCache = new Map();
  function segment(transformId) {
    if (segmentCache.has(transformId)) return segmentCache.get(transformId);
    const transform = transforms.get(transformId);
    if (!transform) return `Transform_${transformId}`;
    const item = gameObjects.get(transform.gameObject);
    const name = item ? item.name : `GameObject_${transform.gameObject}`;
    if (transform.father === '0' || !transforms.has(transform.father)) {
      segmentCache.set(transformId, name);
      return name;
    }
    const parent = transforms.get(transform.father);
    const sameName = parent.children.filter((childId) => {
      const child = transforms.get(childId);
      const childObject = child ? gameObjects.get(child.gameObject) : null;
      return (childObject ? childObject.name : '') === name;
    });
    const ordinal = sameName.indexOf(transformId);
    const result = sameName.length > 1 ? `${name}[${Math.max(0, ordinal)}]` : name;
    segmentCache.set(transformId, result);
    return result;
  }

  const pathCache = new Map();
  function transformPath(transformId) {
    if (pathCache.has(transformId)) return pathCache.get(transformId);
    const transform = transforms.get(transformId);
    if (!transform || transform.father === '0' || !transforms.has(transform.father)) {
      const root = segment(transformId);
      pathCache.set(transformId, root);
      return root;
    }
    const result = `${transformPath(transform.father)}/${segment(transformId)}`;
    pathCache.set(transformId, result);
    return result;
  }

  const gameObjectPath = new Map();
  for (const [transformId, transform] of transforms.entries()) {
    gameObjectPath.set(transform.gameObject, transformPath(transformId));
  }

  const visualGroups = new Map();
  for (const doc of parsed.docs) {
    if (!/^\s*m_(Sprite|Texture):/m.test(doc.body) || !/^\s*m_Color:/m.test(doc.body)) continue;
    const gameObject = fileIdReference(doc.body, 'm_GameObject');
    const objectPath = gameObjectPath.get(gameObject);
    if (!objectPath) continue;
    const script = field(doc.body, 'm_Script') || '';
    const scriptGuid = (script.match(/guid:\s*([0-9a-f]+)/i) || [null, 'builtin'])[1];
    const baseKey = `${objectPath}|${doc.classId}|${scriptGuid}`;
    const group = visualGroups.get(baseKey) || [];
    group.push(doc);
    visualGroups.set(baseKey, group);
  }

  return { visualGroups };
}

function replaceField(targetBody, sourceBody, name) {
  const escaped = name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const sourceMatch = sourceBody.match(new RegExp(`^\\s*${escaped}:.*$`, 'm'));
  if (!sourceMatch) return targetBody;
  const targetRegex = new RegExp(`^\\s*${escaped}:.*$`, 'm');
  return targetRegex.test(targetBody) ? targetBody.replace(targetRegex, sourceMatch[0]) : targetBody;
}

function transplant(sourceText, targetText) {
  const source = parseDocuments(sourceText);
  const target = parseDocuments(targetText);
  const sourceIndex = makeIndex(source);
  const targetIndex = makeIndex(target);
  const fields = ['m_Sprite', 'm_Texture', 'm_Color', 'm_Material', 'm_Type', 'm_PreserveAspect'];
  let matched = 0;

  for (const [key, targetDocs] of targetIndex.visualGroups.entries()) {
    const sourceDocs = sourceIndex.visualGroups.get(key);
    if (!sourceDocs) continue;
    const count = Math.min(targetDocs.length, sourceDocs.length);
    for (let index = 0; index < count; index += 1) {
      for (const name of fields) {
        targetDocs[index].body = replaceField(targetDocs[index].body, sourceDocs[index].body, name);
      }
      matched += 1;
    }
  }

  return {
    text: target.prefix + target.docs.map((doc) => doc.header + doc.body).join(''),
    matched,
    sourceVisuals: [...sourceIndex.visualGroups.values()].reduce((sum, docs) => sum + docs.length, 0),
    targetVisuals: [...targetIndex.visualGroups.values()].reduce((sum, docs) => sum + docs.length, 0),
  };
}

function walk(directory) {
  const results = [];
  for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
    const fullPath = path.join(directory, entry.name);
    if (entry.isDirectory()) results.push(...walk(fullPath));
    else if (entry.isFile() && entry.name.endsWith('.prefab')) results.push(fullPath);
  }
  return results;
}

const rows = [];
for (const sourcePath of walk(rollbackRoot)) {
  const relative = path.relative(rollbackRoot, sourcePath);
  const targetPath = path.join(assetsRoot, relative);
  if (!fs.existsSync(targetPath)) continue;
  const sourceText = fs.readFileSync(sourcePath, 'utf8');
  const targetText = fs.readFileSync(targetPath, 'utf8');
  const result = transplant(sourceText, targetText);
  fs.writeFileSync(targetPath, result.text, 'utf8');
  const hash = crypto.createHash('sha256').update(result.text).digest('hex');
  rows.push({ relative: relative.replace(/\\/g, '/'), ...result, hash });
}

const report = [
  '# Structure-Locked Visual Restore Report',
  '',
  '- Skeleton source: pre-reskin prefab backup',
  '- Visual source: last modified prefab rollback',
  '- Transplanted: Sprite/Texture, Color, Material, Image Type, Preserve Aspect',
  '- Preserved: RectTransform, anchors, pivots, positions, sizes, hierarchy, active state, scripts, references, and interactions',
  '',
  '| Prefab | Matched visual components | Original visual components | Modified visual components | SHA-256 |',
  '|---|---:|---:|---:|---|',
  ...rows.map((row) => `| ${row.relative} | ${row.matched} | ${row.targetVisuals} | ${row.sourceVisuals} | ${row.hash} |`),
  '',
  `Total prefabs: ${rows.length}`,
  `Total matched visual components: ${rows.reduce((sum, row) => sum + row.matched, 0)}`,
  '',
].join('\n');
fs.writeFileSync(reportPath, report, 'utf8');
console.log(`prefabs=${rows.length}`);
console.log(`matched_visual_components=${rows.reduce((sum, row) => sum + row.matched, 0)}`);
console.log(`report=${reportPath}`);
