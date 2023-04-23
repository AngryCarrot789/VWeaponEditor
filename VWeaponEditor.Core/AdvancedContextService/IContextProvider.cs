using System.Collections.Generic;
using VWeaponEditor.Core.AdvancedContextService.Base;

namespace VWeaponEditor.Core.AdvancedContextService {
    public interface IContextProvider {
        List<IContextEntry> GetContext(List<IContextEntry> list);
    }
}