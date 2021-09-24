using System.Collections.Generic;

namespace masterCore.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string IdMenu { get; set; }

        public string DisplayName { get; set; }

        public string IconName { get; set; }

        public string Route { get; set; }

        public string ParentIdMenu { get; set; }

        public virtual List<MenuItem> SubMenuItems { get; set; }
    }
}
