using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AsmAppDev.Models.ViewModels
{
    public class JobVM
    {
        [ValidateNever]
        public Job Job { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
        [ValidateNever]
        public JobApplication apply { get; set; }
    }
}
