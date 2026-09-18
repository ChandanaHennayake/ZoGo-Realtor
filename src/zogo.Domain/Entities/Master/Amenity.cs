using System;
using System.Collections.Generic;
using System.Text;

namespace zogo.Domain.Entities.Master
{

    public class Amenity
    {
        private Amenity()
        {
        }

        public int Id { get; private set; }

        public string Code { get; private set; } = null!;

        public string Name { get; private set; } = null!;

        public string? Category { get; private set; }

        public bool IsActive { get; private set; }

        public int DisplayOrder { get; private set; }
    }
}
