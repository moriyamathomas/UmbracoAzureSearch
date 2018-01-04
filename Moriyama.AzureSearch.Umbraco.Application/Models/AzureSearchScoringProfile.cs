using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;

namespace Moriyama.AzureSearch.Umbraco.Application.Models
{

    public class AzureSearchScoringProfile
	{
		public string Name { get; set; }
        public Dictionary<string, double> TextWeights { get; set; }
        public List<AzureSearchScoringFunction> Functions { get; set; }

		public ScoringProfile GetEffectiveScoringProfile()
		{
			return new ScoringProfile()
			{
				Name = this.Name,
				TextWeights = new TextWeights(this.TextWeights),
				Functions = this.Functions?.Select(x => x.GetEffectiveScoringFunction()).ToList()
			};
		}
	}		
}
