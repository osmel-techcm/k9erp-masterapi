using System.Collections.Generic;

namespace masterCore.DTOs
{
    public class MenuItemDTO
    {
        public int Id { get; set; }

        public string IdMenu { get; set; }

        public string DisplayName { get; set; }

        public string IconName { get; set; }

        public string Route { get; set; }

        public string ParentIdMenu { get; set; }

        public int IdMenuItemUserGroup { get; set; }

        public bool Active { get; set; }

        public virtual List<MenuItemDTO> SubMenuItems { get; set; }
    }
}
