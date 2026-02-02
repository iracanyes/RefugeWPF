using RefugeWPF.CoucheMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RefugeWPF.CoucheMetiers.Model.DTO
{
    class AnimalCompatibilityDTO
    {
        public AnimalCompatibilityDTO(Compatibility compatibility, string animalId, bool? value = null, string description = "")
        {
            Compatibility = compatibility;
            CompatibilityId = compatibility.Id;

            Value = value;
            Description = description;
            AnimalId = animalId != "" ? animalId : null;
        }

        public bool? Value { get; set; }

        public string? Description { get; set; }

        public string? AnimalId { get; set; }



        public Guid CompatibilityId { get; set; }
        public Compatibility Compatibility { get; set; }
    }
}
