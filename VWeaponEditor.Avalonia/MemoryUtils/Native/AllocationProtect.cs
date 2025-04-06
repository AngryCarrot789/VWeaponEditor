namespace VWeaponEditor.Avalonia.MemoryUtils.Native {
    public enum AllocationProtect : uint {
        PAGE_EXECUTE           = 0x010,
        PAGE_EXECUTE_READ      = 0x020,
        PAGE_EXECUTE_READWRITE = 0x040,
        PAGE_EXECUTE_WRITECOPY = 0x080,
        PAGE_NOACCESS          = 0x001,
        PAGE_READONLY          = 0x002,
        PAGE_READWRITE         = 0x004,
        PAGE_WRITECOPY         = 0x008,
        PAGE_GUARD             = 0x100,
        PAGE_NOCACHE           = 0x200,
        PAGE_WRITECOMBINE      = 0x400
    }
}