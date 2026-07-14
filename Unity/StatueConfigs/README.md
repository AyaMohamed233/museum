# StatueConfig Assets Setup

## Overview

StatueConfig is a ScriptableObject that stores configuration for each statue.
The JSON files here are reference templates - actual assets must be created in Unity Editor.

## Fields

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| statueId | string | "default" | Must match instruction file name in gateway/instructions/ |
| displayName | string | - | Display name for UI |
| volume | float | 1.0 | Audio volume (0-1) |
| activeZoneRadius | float | 2.5 | Radius for conversation activation (meters) |
| awarenessZoneRadius | float | 4.0 | Radius for visual indicator (meters) |
| description | string | - | Description text |

## Creating Assets in Unity

1. In Project window, right-click
2. Select: Create > Voice > StatueConfig
3. Name it to match the statue ID (e.g., ramesses_ii)
4. Fill in the Inspector fields

## Statue ID Matching

The statueId must match the instruction file name:

```
Unity StatueConfig.statueId    Gateway Instruction File
ramesses_ii                    gateway/instructions/ramesses_ii.txt
sekhmet                        gateway/instructions/sekhmet.txt
amenhotep_iii                  gateway/instructions/amenhotep_iii.txt
```

## Available Statues

| Statue ID | Display Name |
|-----------|-------------|
| ramesses_ii | Ramesses II |
| amenhotep_iii | Amenhotep III |
| ramesses_iii | Ramesses III |
| sekhmet | Sekhmet |
| serapis | Serapis |
| default | Default Guide |

## Zone Settings

- Active Zone (2.5m): Player can start conversation
- Awareness Zone (4m): Visual indicator shows
- Minimum distance between statues: 8m
