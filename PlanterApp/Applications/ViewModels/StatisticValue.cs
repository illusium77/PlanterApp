using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;

namespace PlanterApp.Applications.ViewModels
{
    internal abstract class StatisticValue : Model
    {
        public enum StatisticType
        {
            General,
            Architecture
        };

        protected readonly IEnumerable<PlantViewModel> _plantModels;
        protected readonly int _plantCount;
        protected int _germinatedCount;

        private StatisticType _type;
        public StatisticType Type
        {
            get { return _type; }
        }

        private object _value;
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value); }
        }

        protected IEnumerable<Plant> PlantsWithArchitecture
        {
            get
            {
                return from model in _plantModels
                       where model.HasArchitecture && model.Plant.PlantArchitecture.Root.Nodes != null && model.Plant.PlantArchitecture.Root.Nodes.Count > 0
                       select model.Plant;
            }
        }

        public StatisticValue(IEnumerable<PlantViewModel> plantModels, StatisticType type)
        {
            _plantModels = plantModels;
            _type = type;

            _plantCount = plantModels.Count();
        }

        protected int StateCount(PlantState state)
        {
            return _plantModels.Count(model => model.HasState(state) && model.Plant.IsExcluded == false);
        }

        protected float? GetPercent(int a, int b)
        {
            if (b != 0)
            {
                return ((float)a / b) * 100;
            }

            return null;
        }

        public abstract void Update();
    }

    internal class GerminatedStatisticValue : StatisticValue
    {
        public GerminatedStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.General)
        {
        }

        public override void Update()
        {
            var germinatedCount = StateCount(PlantState.Alive);

            Value = germinatedCount;
            ToolTip = "Out of " + _plantCount + " plants, " + germinatedCount
                + " have germinated (" + GetPercent(germinatedCount, _plantCount) + "%).";
        }
    }

    internal class AliveStatisticValue : StatisticValue
    {
        public AliveStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.General)
        {
        }

        public override void Update()
        {
            var germinatedCount = StateCount(PlantState.Alive);
            var deadCount = StateCount(PlantState.Dead);

            Value = germinatedCount - deadCount;
            ToolTip = "Out of " + germinatedCount + " germinated plants, " + deadCount
                + " have died (" + GetPercent(deadCount, germinatedCount) + "%).";
        }
    }

    internal class BudsStatisticValue : StatisticValue
    {
        public BudsStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.General)
        {
        }

        public override void Update()
        {
            var germinatedCount = StateCount(PlantState.Alive);
            var budCount = StateCount(PlantState.Buds);
            var budPercent = GetPercent(budCount, germinatedCount);

            Value = budPercent;
            ToolTip = "Out of " + germinatedCount + " germinated plants, " + budCount + " have had buds (" + budPercent + "%).";
        }
    }

    internal class FlowerStatisticValue : StatisticValue
    {
        public FlowerStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.General)
        {
        }

        public override void Update()
        {
            var germinatedCount = StateCount(PlantState.Alive);
            var flowerCount = StateCount(PlantState.Flowering);
            var flowerPercent = GetPercent(flowerCount, germinatedCount);

            Value = flowerPercent;
            ToolTip = "Out of " + germinatedCount + " germinated plants, " + flowerCount + " have had flowers (" + flowerPercent + "%).";
        }
    }

    internal abstract class MeanStatisticValue : StatisticValue
    {
        public MeanStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.General)
        {
        }

        protected void CalculateMeanValue(PlantState startState, PlantState endState, bool includeTransplanted)
        {
            var modelsWithBothStates = (from model in _plantModels
                                       where model.Plant.IsExcluded == false
                                       && model.HasState(startState)
                                       && model.HasState(endState)
                                       && (includeTransplanted ? true : model.Plant.IsTransplanted == false)
                                       select model).ToList();
            
            int countDays = 0;

            var builder = new StringBuilder();
            builder.Append("Mean days from state '" + startState + "' to state '" + endState + "':" + Environment.NewLine);

            if (modelsWithBothStates.Count == 0)
            {
                builder.Append("N/A");
                Value = null;
            }
            else
            {
                builder.Append("(");

                foreach (var model in modelsWithBothStates)
                {
                    var days = model.GetDays(startState, endState);

                    countDays += days;
                    builder.Append(days + "+");
                }

                builder.Remove(builder.Length - 1, 1);

                Value = (float)countDays / modelsWithBothStates.Count;
                builder.Append(") / " + modelsWithBothStates.Count + " = " + Value);

                if (includeTransplanted == false)
                {
                    builder.Append(Environment.NewLine + "(Transplanted plants have been excluded from calculations)");
                }
            }

            ToolTip = builder.ToString();
        }
    }

    internal class MeanGerminationStatisticValue : MeanStatisticValue
    {
        public MeanGerminationStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels)
        {
        }

        public override void Update()
        {
            CalculateMeanValue(PlantState.Planted, PlantState.Alive, false);
        }
    }

    internal class MeanBudsStatisticValue : MeanStatisticValue
    {
        public MeanBudsStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels)
        {
        }

        public override void Update()
        {
            CalculateMeanValue(PlantState.Planted, PlantState.Buds, true);
        }
    }

    internal class MeanFloweringStatisticValue : MeanStatisticValue
    {
        public MeanFloweringStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels)
        {
        }

        public override void Update()
        {
            CalculateMeanValue(PlantState.Planted, PlantState.Flowering, true);
        }
    }

    internal class MeanSeedsStatisticValue : MeanStatisticValue
    {
        public MeanSeedsStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels)
        {
        }

        public override void Update()
        {
            CalculateMeanValue(PlantState.Planted, PlantState.Seeds, true);
        }
    }

    internal class MeanFloweringSeedsStatisticValue : MeanStatisticValue
    {
        public MeanFloweringSeedsStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels)
        {
        }

        public override void Update()
        {
            CalculateMeanValue(PlantState.Flowering, PlantState.Seeds, true);
        }
    }

    internal class MeanBudsFloweringStatisticValue : MeanStatisticValue
    {
        public MeanBudsFloweringStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels)
        {
        }

        public override void Update()
        {
            CalculateMeanValue(PlantState.Buds, PlantState.Flowering, true);
        }
    }

    internal class MeristemCountStatisticValue : StatisticValue
    {
        public MeristemCountStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.Architecture)
        {
        }

        public override void Update()
        {
            var numTotal = 0; // number of meristems

            foreach (var plant in PlantsWithArchitecture)
            {
                numTotal += plant.PlantArchitecture.FindMeristems(MeristemType.Branch, MeristemType.Flower, MeristemType.TBD).Count();
            }

            Value = numTotal;
            ToolTip = "Total number of meristems: " + numTotal;
        }
    }

    internal class StemStatisticValue : StatisticValue
    {
        public StemStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.Architecture)
        {
        }

        public override void Update()
        {
            var plantsWithArchitecture = PlantsWithArchitecture;
            var plantsWithStemCount = plantsWithArchitecture.Count(plant => plant.PlantArchitecture.FindNodes(NodeType.Stem).Count() > 0);
            var stemPercentage = GetPercent(plantsWithStemCount, plantsWithArchitecture.Count());

            Value = stemPercentage;
            ToolTip = "Out of " + plantsWithArchitecture.Count() + " plants with architecture, " + plantsWithStemCount + " has stems (" + stemPercentage + "%).";
        }
    }

    internal class BranchStatisticValue : StatisticValue
    {
        public BranchStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.Architecture)
        {
        }

        public override void Update()
        {
            var plantsWithArchitecture = PlantsWithArchitecture;
            var plantsWithBranchCount = plantsWithArchitecture.Count(plant => plant.PlantArchitecture.FindMeristems(MeristemType.Branch).Count() > 0);
            var branchPercentage = GetPercent(plantsWithBranchCount, plantsWithArchitecture.Count());

            Value = branchPercentage;
            ToolTip = "Out of " + plantsWithArchitecture.Count() + " plants with architecture, " + plantsWithBranchCount + " has branches (" + branchPercentage + "%).";
        }
    }

    internal class SecondaryStatisticValue : StatisticValue
    {
        public SecondaryStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.Architecture)
        {
        }

        public override void Update()
        {
            var plantsWithArchitecture = PlantsWithArchitecture;
            var plantsWithSecondaryCount = plantsWithArchitecture.Count(plant => plant.PlantArchitecture.FindNodes(NodeType.Secondary).Count() > 0);
            var secondaryPercentage = GetPercent(plantsWithSecondaryCount, plantsWithArchitecture.Count());

            Value = secondaryPercentage;
            ToolTip = "Out of " + plantsWithArchitecture.Count() + " plants with architecture, " + plantsWithSecondaryCount + " has secondary nodes (" + secondaryPercentage + "%).";
        }
    }

    internal class BasalBranchStatisticValue : StatisticValue
    {
        public BasalBranchStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.Architecture)
        {
        }

        public override void Update()
        {
            var numBranch = 0;
            var numTotal = 0; // number of meristem in basals & cotyledons

            foreach (var plant in PlantsWithArchitecture)
            {
                var meristems = (from node in plant.PlantArchitecture.FindNodes(NodeType.Basal, NodeType.Cotyledons)
                                 select node.Meristems).SelectMany(m => m);

                numBranch += meristems.Count(meristem => meristem.Type == MeristemType.Branch);
                numTotal += meristems.Count();
            }

            Value = GetPercent(numBranch, numTotal);
            ToolTip = "Basal (and Cotyledons) Branch Percentage:" + Environment.NewLine
                + "Out of " + numTotal + " meristems, " + numBranch + " has branch (" + Value + "%).";
        }
    }

    internal class FlowerMeristemStatisticValue : StatisticValue
    {
        public FlowerMeristemStatisticValue(IEnumerable<PlantViewModel> plantModels)
            : base(plantModels, StatisticType.Architecture)
        {
        }

        public override void Update()
        {
            var numFlowers = 0;
            var numTotal = 0; // number of meristem in basals & cotyledons

            foreach (var plant in PlantsWithArchitecture)
            {
                numFlowers += plant.PlantArchitecture.FindMeristems(MeristemType.Flower).Count();
                numTotal += plant.PlantArchitecture.FindMeristems(MeristemType.Branch, MeristemType.Flower, MeristemType.TBD).Count();
            }

            Value = GetPercent(numFlowers, numTotal);
            ToolTip = "Flower Meristem Percentage:" + Environment.NewLine
                + "Out of " + numTotal + " meristems, " + numFlowers + " has flowers (" + Value + "%).";
        }
    }
}
