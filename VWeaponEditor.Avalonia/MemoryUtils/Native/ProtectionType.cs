namespace VWeaponEditor.Avalonia.MemoryUtils.Native;

public enum ProtectionType : uint {
    Read = 0x0010,
    Write = 0x0020,
    All = 0x1F0FFF
}