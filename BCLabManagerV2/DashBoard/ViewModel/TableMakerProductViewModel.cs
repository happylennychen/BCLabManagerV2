using BCLabManager.Model;
using System.Collections.Generic;
using System.Linq;

namespace BCLabManager.ViewModel
{
    public class TableMakerProductViewModel
    {
        private TableMakerProduct _product;
        public TableMakerProductViewModel(TableMakerProduct product)
        {
            _product = product;
        }
        public string Type
        {
            get
            {
                return _product.Type.Description;
            }
        }
        public string IsEvaluated
        {
            get
            {
                if (_product == null)
                    return string.Empty;
                if (_product.Project == null)
                    return string.Empty;
                if (_product.Project.EmulatorResults == null)
                    return string.Empty;
                var ers = _product.Project.EmulatorResults.Where(er => er.is_valid).ToList();
                List<EmulatorResult> pd_ers;
                if (_product.Type.Description.Contains("C file"))
                    pd_ers = ers.Where(er => er.table_maker_cfile == _product).ToList();
                else if (_product.Type.Description.Contains("H file"))
                    pd_ers = ers.Where(er => er.table_maker_hfile == _product).ToList();
                else
                    return "";
                var recs = _product.Project.Programs.Where(pro => pro.Type.Name == "EV").SelectMany(pro => pro.Recipes.Where(rec => rec.IsCompleted)).ToList();
                if (pd_ers.Count == recs.Count)
                    return "PASS";
                else
                    return "";
            }
        }
    }
}