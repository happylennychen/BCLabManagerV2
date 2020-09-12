using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace BCLabManager
{
    internal class PseudocodeProcesser
    {
        internal static void Load(string fileName, RecipeTemplateServiceClass recipeTemplateService, ProgramServiceClass programService, ProjectServiceClass projectService, ProgramTypeServiceClass programTypeService)
        {
            List<RecipeTemplate> recipeTemplates = BuildRecipeTemplatesFromPseudocode(fileName, recipeTemplateService);
            if (recipeTemplates != null)
                foreach (var rt in recipeTemplates)
                    recipeTemplateService.SuperAdd(rt);

            List<Program> programs = BuildProgramsFromPseudocode(fileName, programService, projectService, programTypeService, recipeTemplateService);
            if (programs != null)
                foreach (var prog in programs)
                {
                    programService.SuperAdd(prog);
                    //programService.FixTemplates(prog);    //不管用
                }
        }

        private static List<RecipeTemplate> BuildRecipeTemplatesFromPseudocode(string fileName, RecipeTemplateServiceClass recipeTemplateService)
        {
            List<RecipeTemplate> output = new List<RecipeTemplate>();
            if (File.Exists(fileName))
            {
                XDocument xd = new XDocument();
                xd = XDocument.Load(fileName);
                var xmlrecTemplates = xd.Descendants("RecipeTemplates").Elements();
                foreach (var xmlrecTemplate in xmlrecTemplates)
                {
                    output.Add(BuildRecipeTemplateFromPseudocode(xmlrecTemplate));
                }
            }
            if (output.Count != 0)
                return output;
            else
                return null;
        }

        private static RecipeTemplate BuildRecipeTemplateFromPseudocode(XElement xmlrecTemplate)
        {
            var recTemplate = new RecipeTemplate();

            recTemplate.Name = xmlrecTemplate.Attribute("Name").Value;

            var xmlprotections = xmlrecTemplate.Descendants("Protections").Elements();
            if (xmlprotections != null)
            {
                foreach (var xmlprotection in xmlprotections)
                {
                    var protection = new Protection();
                    protection.Parameter = GetParameterFromNode(xmlprotection.Attribute("Parameter").Value);
                    protection.Mark = GetMarkFromNode(xmlprotection.Attribute("Mark").Value);
                    protection.Value = Convert.ToInt32(xmlprotection.Attribute("Value").Value);
                    recTemplate.Protections.Add(protection);
                }
            }

            var xmlsteps = xmlrecTemplate.Descendants("Steps").Elements();
            if (xmlsteps != null)
            {
                foreach (var xmlstep in xmlsteps)
                {
                    var step = new StepV2();
                    step.Index = GetIntergerFromNode(xmlstep, "Index");
                    step.Rest = GetIntergerFromNode(xmlstep, "Rest");
                    step.Prerest = GetIntergerFromNode(xmlstep, "Prerest");
                    step.Loop1Label = GetStringFromNode(xmlstep, "Loop1Label");
                    step.Loop2Label = GetStringFromNode(xmlstep, "Loop2Label");

                    var xmlaction = xmlstep.Element("Action");
                    if (xmlaction != null)
                    {
                        var action = new TesterAction();
                        action.Mode = GetModeFromNode(xmlaction.Attribute("Mode").Value);
                        action.Voltage = GetIntergerFromNode(xmlaction, "Voltage");
                        action.Current = GetIntergerFromNode(xmlaction, "Current");
                        action.Power = GetIntergerFromNode(xmlaction, "Power");
                        step.Action = action;
                    }

                    var xmlCutOffConditions = xmlstep.Descendants("CutOffConditions").Elements();
                    if (xmlCutOffConditions != null)
                    {
                        foreach (var xmlCOC in xmlCutOffConditions)
                        {
                            var coc = new CutOffCondition();
                            coc.Parameter = GetParameterFromNode(xmlCOC.Attribute("Parameter").Value);
                            coc.Mark = GetMarkFromNode(xmlCOC.Attribute("Mark").Value);
                            coc.Value = Convert.ToInt32(xmlCOC.Attribute("Value").Value);
                            coc.JumpType = GetJumpTypeFromNode(xmlCOC.Attribute("JumpType").Value);
                            coc.Index = GetIntergerFromNode(xmlCOC, "Index");
                            coc.Loop1Target = GetStringFromNode(xmlstep, "Loop1Target");
                            coc.Loop1Count = Convert.ToUInt16(GetIntergerFromNode(xmlCOC, "Loop1Count"));
                            coc.Loop2Target = GetStringFromNode(xmlstep, "Loop2Target");
                            coc.Loop2Count = Convert.ToUInt16(GetIntergerFromNode(xmlCOC, "Loop2Count"));

                            step.CutOffConditions.Add(coc);
                        }
                    }
                    recTemplate.StepV2s.Add(step);
                }
            }
            return recTemplate;
        }

        private static JumpType GetJumpTypeFromNode(string value)
        {
            JumpType jt = JumpType.NA;
            switch (value.ToUpper())
            {
                case "NEXT":
                    jt = JumpType.NEXT;
                    break;
                case "END":
                    jt = JumpType.END;
                    break;
                case "INDEX":
                    jt = JumpType.INDEX;
                    break;
                case "LOOP":
                    jt = JumpType.LOOP;
                    break;
            }
            return jt;
        }

        private static ActionMode GetModeFromNode(string value)
        {
            ActionMode m = ActionMode.NA;
            switch (value.ToUpper())
            {
                case "CC-CV CHARGE":
                    m = ActionMode.CC_CV_CHARGE;
                    break;
                case "CC DISCHARGE":
                    m = ActionMode.CC_DISCHARGE;
                    break;
                case "CP DISCHARGE":
                    m = ActionMode.CP_DISCHARGE;
                    break;
                case "REST":
                    m = ActionMode.REST;
                    break;
                default:
                    m = ActionMode.NA;
                    break;
            }
            return m;
        }

        private static string GetStringFromNode(XElement xe, string v)
        {
            if (xe.Attribute(v) != null)
                return xe.Attribute(v).Value;
            else
                return string.Empty;
        }

        private static int GetIntergerFromNode(XElement xe, string v)
        {
            if (xe.Attribute(v) != null && xe.Attribute(v).Value != string.Empty)
                return Convert.ToInt32(xe.Attribute(v).Value);
            else
                return 0;
        }

        private static CompareMarkEnum GetMarkFromNode(string value)
        {
            CompareMarkEnum m = CompareMarkEnum.NA;
            switch (value.ToUpper())
            {
                case "LARGERTHAN":
                    m = CompareMarkEnum.LargerThan;
                    break;
                case "SMALLERTHAN":
                    m = CompareMarkEnum.SmallerThan;
                    break;
                case "EQUALTO":
                    m = CompareMarkEnum.EqualTo;
                    break;
            }
            return m;
        }

        private static Parameter GetParameterFromNode(string value)
        {
            Parameter p = Parameter.NA;
            switch (value.ToUpper())
            {
                case "VOLTAGE":
                    p = Parameter.VOLTAGE;
                    break;
                case "CURRENT":
                    p = Parameter.CURRENT;
                    break;
                case "TEMPERATURE":
                    p = Parameter.TEMPERATURE;
                    break;
                case "POWER":
                    p = Parameter.POWER;
                    break;
                case "TIME":
                    p = Parameter.TIME;
                    break;
            }
            return p;
        }

        private static List<Program> BuildProgramsFromPseudocode(string fileName, ProgramServiceClass programService, ProjectServiceClass projectService, ProgramTypeServiceClass programTypeService, RecipeTemplateServiceClass recipeTemplateService)
        {
            List<Program> output = new List<Program>();
            if (File.Exists(fileName))
            {
                XDocument xd = new XDocument();
                xd = XDocument.Load(fileName);
                var xmlPrograms = xd.Descendants("Programs").Elements();
                foreach (var xmlProgram in xmlPrograms)
                {
                    output.Add(BuildProgramFromPseudocode(xmlProgram, programService, projectService, programTypeService, recipeTemplateService));
                }
            }
            if (output.Count != 0)
                return output;
            else
                return null;
        }

        private static Program BuildProgramFromPseudocode(XElement xmlProgram, ProgramServiceClass programService, ProjectServiceClass projectService, ProgramTypeServiceClass programTypeService, RecipeTemplateServiceClass recipeTemplateService)
        {
            var program = new Program();
            try
            {

                program.Name = xmlProgram.Attribute("Name").Value;
                program.Type = GetProgramTypeFromNode(xmlProgram.Attribute("Type").Value, programTypeService);
                program.Project = GetProjectFromNode(xmlProgram.Attribute("Project").Value, projectService);
                program.Requester = xmlProgram.Attribute("Requester").Value;
                program.RequestTime = DateTime.Parse(xmlProgram.Attribute("RequestDate").Value);

                var xmlTemperatures = xmlProgram.Descendants("Temperatures").Elements();
                if (xmlTemperatures != null)
                {
                    foreach (var xmlT in xmlTemperatures)
                    {
                        program.Temperatures.Add(Convert.ToInt32(xmlT.Value));
                    }
                }

                var xmlRecipes = xmlProgram.Descendants("Recipes").Elements();
                if (xmlRecipes != null)
                {
                    var dic = new Dictionary<string, int>();
                    foreach (var xmlRec in xmlRecipes)
                    {
                        if (xmlRec.Attribute("Template") == null)
                            continue;
                        var recTname = xmlRec.Attribute("Template").Value;
                        if (recTname == string.Empty)
                            continue;
                        var recT = recipeTemplateService.Items.Last(o => o.Name == recTname);
                        if (recT == null)
                        {
                            MessageBox.Show($@"No such recipe template:{recTname}");
                            continue;
                        }
                        program.RecipeTemplates.Add(recT.Name);
                        var count = GetIntergerFromNode(xmlRec, "Count");
                        if (count == 0)
                            count = 1;
                        dic.Add(recT.Name, count);
                    }
                    program.BuildRecipes(recipeTemplateService.Items.ToList(), dic);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return program;
        }

        private static Project GetProjectFromNode(string value, ProjectServiceClass projectService)
        {
            return projectService.Items.SingleOrDefault(o => o.Name == value);
        }

        private static ProgramType GetProgramTypeFromNode(string value, ProgramTypeServiceClass programTypeService)
        {
            return programTypeService.Items.SingleOrDefault(o => o.Name == value);
        }
    }
}