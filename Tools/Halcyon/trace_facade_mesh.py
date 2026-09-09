"""Read the illustration to derive sprite geometry; never rewrite bitmap pixels."""
from collections import deque
from pathlib import Path
import json
import numpy as np
from PIL import Image

folder = Path(__file__).resolve().parents[2] / 'Assets/HalcyonSlice/Resources/HalcyonAcademy'
pixels = np.asarray(Image.open(folder / 'AcademyBuilding.png').convert('RGB')).astype(np.int16)
h, w, _ = pixels.shape
background = (pixels.min(axis=2) > 166) & ((pixels.max(axis=2) - pixels.min(axis=2)) < 34)
outside = np.zeros((h, w), dtype=bool)
queue = deque()
def add(x, y):
    if 0 <= x < w and 0 <= y < h and not outside[y, x] and background[y, x]:
        outside[y, x] = True
        queue.append((x, y))
for x in range(w):
    add(x, 0); add(x, h-1)
for y in range(h):
    add(0, y); add(w-1, y)
while queue:
    x, y = queue.popleft()
    add(x-1, y); add(x+1, y); add(x, y-1); add(x, y+1)

# Keep the main architecture component, discarding isolated specks in the export.
solid = ~outside
seen = np.zeros((h,w), dtype=bool)
components = []
for y, x in zip(*np.where(solid)):
    if seen[y,x]: continue
    seen[y,x] = True; queue = deque([(int(x),int(y))]); component = []
    while queue:
        px, py = queue.popleft(); component.append((px,py))
        for nx, ny in ((px-1,py),(px+1,py),(px,py-1),(px,py+1)):
            if 0 <= nx < w and 0 <= ny < h and solid[ny,nx] and not seen[ny,nx]:
                seen[ny,nx] = True; queue.append((nx,ny))
    if len(component) > 100: components.append(component)
mask = np.zeros((h,w), dtype=bool)
largest = max(components, key=len)
for x,y in largest: mask[y,x] = True
spans = []
for y in range(h):
    changes = np.diff(np.concatenate(([False], mask[y], [False])).astype(np.int8))
    for begin, end in zip(np.where(changes == 1)[0], np.where(changes == -1)[0]):
        spans.extend((y, int(begin), int(end)))
(folder / 'AcademyFacadeMesh.json').write_text(json.dumps({'width':w,'height':h,'spans':spans}, separators=(',',':')) + '\n')
print(f'Architecture mesh: {len(spans)//3} horizontal strips, {len(largest)} source pixels covered; bitmap unchanged.')
