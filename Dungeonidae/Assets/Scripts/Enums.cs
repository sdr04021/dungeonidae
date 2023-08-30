public enum AdditionalEffectKey { BLOCK, COMBO, MISSILE }
public enum AreaType { Room, Hallway, Entrance, Border, None }
public enum AttackType { Atk, MAtk, Fixed}
public enum BuffType { Buff, Debuff }
public enum DamageType { Normal, Miss, Critical, Block, Heal }
public enum Directions { N, NE, E, SE, S, SW, W, NW, NONE }
public enum DungeonObjectDurability { Fragile, Breakable, Unbreakable }
public enum DungeonType { Dungeon, Maze }
public enum EquipmentType { Hat, Weapon, Armor, Sub, Shoes, Artifact }
public enum ItemSlotType { Item, Equipped, Shop }
public enum ItemType { Equipment, Misc }
public enum LayerOrder { BottomWall, Stair, DungeonObject, Unit, ItemObject, Bush, Wall, TopWall, Fog, Canvas }
public enum PrefabAssetType { Monster, DungeonObject, ParticleEffect}
public enum SkillType { Attack, Status }
public enum SpriteAssetType { Ability, Equipment, Misc, Skill }
public enum StatType { MaxHp, MaxMp, MaxHunger, Atk, Def, MAtk, MDef, Pen, MPen, Proficiency, Acc, Eva, Cri, CriDmg, Aspd,
    AtkRange, HpRegen, MpRegen, LifeSteal, ManaSteal, Resist, CoolSpeed, Sight, Speed, SearchRange, Instinct, Stealth, DmgIncrease, DmgReduction}
public enum StatUnit { Value, Percent }
public enum StatValueType { Original, Additional, Temporary, Percent }
public enum Team { Neutral, Chaotic, Player, Ally, Enemy, Enemy2 }
public enum TileType { Floor, Wall }