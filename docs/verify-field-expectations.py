"""Independent achromatic reference: no Iro import, no result-derived targets.
D65 neutral sRGB -> L*, then CIEDE2000 with delta chroma/hue zero.
Tiny chroma from rounded RGB matrix coefficients is covered by 0.001.
"""
import json
import math
from pathlib import Path

def lightness(code):
    s = code / 255
    y = s / 12.92 if s <= 0.04045 else ((s + 0.055) / 1.055) ** 2.4
    return 116 * y ** (1 / 3) - 16 if y > (6 / 29) ** 3 else y * (29 / 3) ** 3

def neutral_distance(a, b):
    first, second = lightness(a), lightness(b)
    mean = (first + second) / 2
    scale = 1 + 0.015 * (mean - 50) ** 2 / math.sqrt(20 + (mean - 50) ** 2)
    return abs(first - second) / scale

root = Path(__file__).resolve().parents[1]
count = 0
for name in ('feldzuordnung-und-messwerte.json', 'rangfolge-und-gleichstaende.json'):
    plan = json.loads((root / 'iro-gen/testplans' / name).read_text(encoding='utf-8-sig'))
    for case in plan['cases']:
        colors = case['options'].get('colors', plan['defaults']['colors'])
        wall = colors['wall']
        assert wall['r'] == wall['g'] == wall['b']
        references = {}
        for index, pixel in enumerate(colors['palette']):
            assert pixel['r'] == pixel['g'] == pixel['b']
            expected = case['expected']['fields'][index]
            reference = neutral_distance(pixel['r'], wall['r'])
            assert abs(expected['deltaE00'] - reference) <= 0.0000005
            assert expected['fieldId'] == f'field-{index + 1}'
            references[expected['fieldId']] = reference
        ranking = case['expected'].get('ranking')
        if ranking:
            ordered = [(field, rank) for rank, group in enumerate(ranking['groups']) for field in group]
            assert len(ordered) == len(references) == len(set(field for field, _ in ordered))
            for i, (first, first_rank) in enumerate(ordered):
                for second, second_rank in ordered[i+1:]:
                    difference = references[second] - references[first]
                    if first_rank == second_rank:
                        assert abs(difference) <= ranking['tieTolerance']
                    else:
                        assert difference > ranking['tieTolerance']
        count += 1
print(f'Independent neutral values and configured ranking groups verified: {count} cases.')
