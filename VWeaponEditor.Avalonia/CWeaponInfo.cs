using System.Runtime.InteropServices;

namespace VWeaponEditor.Avalonia {
    // public unsafe struct RageATArray<T> {
    //     public long a, b; // 16 bytes
    // }
    
    public unsafe struct RageATArray {
        public long a, b; // 16 bytes
    }

    public enum eDamageType : int {
        Unknown,
        None,
        Melee,
        Bullet,
        BulletRubber,
        Explosive,
        Fire,
        Collision,
        Fall,
        Drown,
        Electric,
        BarbedWire,
        FireExtinguisher,
        Smoke,
        WaterCannon,
        Tranquilizer
    };

    public enum eFireType : int {
        None,
        Melee,
        InstantHit,
        DelayedHit,
        ProjectTile,
        VolumetricParticle
    };

    public enum eWeaponWheelSlot : int {
        Pistol,
        SMG,
        Rifle,
        Sniper,
        UnarmedMelee,
        ShotGun,
        Heavy,
        ThrowableSpecial
    };

    public unsafe struct CAimingInfo {
        public uint m_name_hash; //0x00
        public float m_heading_limit; //0x04
        public float m_sweep_pitch_min; //0x08
        public float m_sweep_pitch_max; //0x0C
    };

    public enum eEffectGroup : int {
        PunchKick,
        MeleeWood,
        MeleeMetal,
        MeleeSharp,
        MeleeGeneric,
        PistolSmall,
        PistolLarge,
        PistolSilenced,
        Rubber,
        SMG,
        ShotGun,
        RifleAssault,
        RifleSniper,
        Rocket,
        Grenade,
        Molotov,
        Fire,
        Explosion,
        Laser,
        Stungun,
        HeavyMG,
        VehicleMG
    }

    public unsafe struct sWeaponFx { //0x00
        public eEffectGroup m_effect_group;
        public uint m_flash_fx_hash; //0x04
        public uint m_flash_fx_alt_hash; //0x08
        public uint m_flash_fx_fp_hash; //0x0C
        public uint m_flash_fx_fp_alt_hash; //0x10
        public uint m_smoke_fx_hash; //0x14
        public uint m_smoke_fx_fp_hash; //0x18
        public float m_muzzle_smoke_fx_min_level; //0x1C
        public float m_muzzle_smoke_fx_inc_per_shot; //0x20
        public float m_muzzle_smoke_fx_dec_per_sec; //0x24
        public fixed byte pad_28[8];
        public Vector3f m_muzzle_override_offset; //0x30
        public fixed byte pad_3C[8];
        public uint m_shell_fx_hash; //0x44
        public uint m_shell_fx_fp_hash; //0x48
        public uint m_tracer_fx_hash; //0x4C
        public uint m_ped_damage_hash; //0x50
        public float m_tracer_fx_chance_sp; //0x54
        public float m_tracer_fx_chance_mp; //0x58
        public fixed byte pad_5C[4];
        public float m_flash_fx_chance_sp; //0x60
        public float m_flash_fx_chance_mp; //0x64
        public float m_flash_fx_alt_chance; //0x68
        public float m_flash_fx_scale; //0x6C
        public bool m_flash_fx_light_enabled; //0x70
        public bool m_flash_fx_light_casts_shadows; //0x71
        public float m_flash_fx_light_offset_dist; //0x74
        public fixed byte pad_78[8];
        public Vector3f m_flash_fx_light_rgba_min; //0x80
        public fixed byte pad_8C[4];
        public Vector3f m_flash_fx_light_rgba_max; //0x90
        public fixed byte pad_9C[4];
        public Vector2f m_flash_fx_light_intensity_minmax; //0xA0
        public Vector2f m_flash_fx_light_range_minmax; //0xA8
        public Vector2f m_flash_fx_light_falloff_minmax; //0xB0
        public bool m_ground_disturb_fx_enabled; // 0xB8
        public float m_ground_disturb_fx_dist; //0xBC
        public uint m_ground_disturb_fx_name_default_hash; //0xC0
        public uint m_ground_disturb_fx_name_sand_hash; //0xC4
        public uint m_ground_disturb_fx_name_dirt_hash; //0xC8
        public uint m_ground_disturb_fx_name_water_hash; //0xCC
        public uint m_ground_disturb_fx_name_foliage_hash; //0xD0
        public fixed byte pad_D4[12];
    };

    public unsafe struct sComponent {
        public uint m_name_hash;
        public bool m_default;
    };

    public unsafe struct CWeaponComponentPoint {
        public uint m_attach_bone_hash; //0x00
        public fixed byte pad_04[4];
        public sComponent m_components_0; // 0x08
        public sComponent m_components_1;
        public sComponent m_components_2;
        public sComponent m_components_3;
        public sComponent m_components_4;
        public sComponent m_components_5;
        public sComponent m_components_6;
        public sComponent m_components_7;
        public sComponent m_components_8;
        public sComponent m_components_9;
        public sComponent m_components_10;
        public sComponent m_components_11;
        public int m_component_count; // 0x68
    };

    public unsafe struct CWeaponSpecValue {
        public float m_spec_fresnel; //0x00
        public float m_spec_falloff_mult; //0x04
        public float m_spec_int_mult; //0x08
        public float m_spec2_factor; //0x0c
        public float m_spec2_color_int; //0x10
        public uint m_spec2_color; //0x14
    };

    public unsafe struct CWeaponTintSpecValues {
        public uint m_name_hash; //0x00
        // public RageATArray<CWeaponSpecValue> m_tints; //0x08
        public RageATArray m_tints; //0x08
    };

    public unsafe struct CFiringPatternAlias {
        public uint m_firing_pattern_hash; //0x00
        public uint m_alias_hash; //0x04
    };

    public unsafe struct CWeaponFiringPatternAliases {
        public uint m_name_hash; //0x00
        // public RageATArray<CFiringPatternAlias> m_aliases; //0x08
        public RageATArray m_aliases; //0x08
    };

    public unsafe struct sData {
        public float m_idle; //0x00
        public float m_walk; //0x04
        public float m_run; //0x08
    };

    public unsafe struct CWeaponUpperBodyFixupExpressionData {
        public uint m_name_hash; //0x00
        public sData m_data_0; //0x04
        public sData m_data_1;
        public sData m_data_2;
        public sData m_data_3;
    };

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CCamoDiffuseTexIdxs {
        [FieldOffset(0)] public uint m_key_hash; //0x00

        public unsafe struct sKeyValue {
            public uint m_key; //0x0
            public uint m_value; //0x4
        };

        // [FieldOffset(10)] public RageATArray<sKeyValue> m_items; //0x10
        [FieldOffset(10)] public RageATArray m_items; //0x10
    };

    public unsafe struct Vector2f {
        public float x, y;

        public override string ToString() {
            return $"{nameof(this.x)}: {this.x}, {nameof(this.y)}: {this.y}";
        }
    }

    public unsafe struct Vector3f {
        public float x, y, z;

        public override string ToString() {
            return $"{nameof(this.x)}: {this.x}, {nameof(this.y)}: {this.y}";
        }
    }

    public unsafe struct sBoneForce {
        public int m_bone_tag; //0x00
        public float m_force_front; //0x04
        public float m_force_back; //0x08
    };

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct sFirstPersonScopeAttachmentData {
        [FieldOffset(0)] public uint m_name_hash; //0x00
        [FieldOffset(4)] public float m_first_person_scope_attachment_fov; //0x04
        [FieldOffset(10)] public Vector3f m_first_person_scope_attachment_offset; //0x10
        [FieldOffset(20)] public Vector3f m_first_person_scope_attachment_rotation_offset; //0x20
    };

    public enum eExplosionTag : int {
        DONTCARE = -1,
        GRENADE,
        GRENADELAUNCHER,
        STICKYBOMB,
        MOLOTOV,
        ROCKET,
        TANKSHELL,
        HI_OCTANE,
        CAR,
        PLANE,
        PETROL_PUMP,
        BIKE,
        DIR_STEAM,
        DIR_FLAME,
        DIR_WATER_HYDRANT,
        DIR_GAS_CANISTER,
        BOAT,
        SHIP_DESTROY,
        TRUCK,
        BULLET,
        SMOKEGRENADELAUNCHER,
        SMOKEGRENADE,
        BZGAS,
        FLARE,
        GAS_CANISTER,
        EXTINGUISHER,
        _0x988620B8,
        EXP_TAG_TRAIN,
        EXP_TAG_BARREL,
        EXP_TAG_PROPANE,
        EXP_TAG_BLIMP,
        EXP_TAG_DIR_FLAME_EXPLODE,
        EXP_TAG_TANKER,
        PLANE_ROCKET,
        EXP_TAG_VEHICLE_BULLET,
        EXP_TAG_GAS_TANK,
        EXP_TAG_BIRD_CRAP,
        EXP_TAG_RAILGUN,
        EXP_TAG_BLIMP2,
        EXP_TAG_FIREWORK,
        EXP_TAG_SNOWBALL,
        EXP_TAG_PROXMINE,
        EXP_TAG_VALKYRIE_CANNON,
        EXP_TAG_AIR_DEFENCE,
        EXP_TAG_PIPEBOMB,
        EXP_TAG_VEHICLEMINE,
        EXP_TAG_EXPLOSIVEAMMO,
        EXP_TAG_APCSHELL,
        EXP_TAG_BOMB_CLUSTER,
        EXP_TAG_BOMB_GAS,
        EXP_TAG_BOMB_INCENDIARY,
        EXP_TAG_BOMB_STANDARD,
        EXP_TAG_TORPEDO,
        EXP_TAG_TORPEDO_UNDERWATER,
        EXP_TAG_BOMBUSHKA_CANNON,
        EXP_TAG_BOMB_CLUSTER_SECONDARY,
        EXP_TAG_HUNTER_BARRAGE,
        EXP_TAG_HUNTER_CANNON,
        EXP_TAG_ROGUE_CANNON,
        EXP_TAG_MINE_UNDERWATER,
        EXP_TAG_ORBITAL_CANNON,
        EXP_TAG_BOMB_STANDARD_WIDE,
        EXP_TAG_EXPLOSIVEAMMO_SHOTGUN,
        EXP_TAG_OPPRESSOR2_CANNON,
        EXP_TAG_MORTAR_KINETIC,
        EXP_TAG_VEHICLEMINE_KINETIC,
        EXP_TAG_VEHICLEMINE_EMP,
        EXP_TAG_VEHICLEMINE_SPIKE,
        EXP_TAG_VEHICLEMINE_SLICK,
        EXP_TAG_VEHICLEMINE_TAR,
        EXP_TAG_SCRIPT_DRONE,
        EXP_TAG_RAYGUN,
        EXP_TAG_BURIEDMINE,
        EXP_TAG_SCRIPT_MISSILE,
        EXP_TAG_RCTANK_ROCKET,
        EXP_TAG_BOMB_WATER,
        EXP_TAG_BOMB_WATER_SECONDARY,
        _0xF728C4A9,
        _0xBAEC056F,
        EXP_TAG_FLASHGRENADE,
        EXP_TAG_STUNGRENADE,
        _0x763D3B3B,
        EXP_TAG_SCRIPT_MISSILE_LARGE,
        EXP_TAG_SUBMARINE_BIG,
        EXP_TAG_EMPLAUNCHER_EMP,
    };

    public unsafe struct sFrontClearTestParams {
        public bool m_should_perform_front_clear_test; //0x0000
        public float m_forward_offset; //0x0004
        public float m_vertical_offset; //0x0008
        public float m_horizontal_offset; //0x000C
        public float m_capsule_radius; //0x0010
        public float m_capsule_length; //0x0014
    }

    public unsafe struct sExplosion {
        public eExplosionTag m_default; //0x0000
        public eExplosionTag m_hit_car; //0x0004
        public eExplosionTag m_hit_truck; //0x0008
        public eExplosionTag m_hit_bike; //0x000C
        public eExplosionTag m_hit_boat; //0x0010
        public eExplosionTag m_hit_plane; //0x0014
    }

    public enum eAmmoSpecialType : int {
        None,
        ArmorPiercing,
        Explosive,
        FMJ,
        HollowPoint,
        Incendiary,
        Tracer
    };

    public enum eAmmoFlags : uint {
        InfiniteAmmo = 0,
        AddSmokeOnExplosion = 1,
        Fuse = 2,
        FixedAfterExplosion = 3,
    };

    public unsafe struct CWeaponBoneId {
        public ushort m_bone_id;
    };
}