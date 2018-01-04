using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;

namespace Moriyama.AzureSearch.Umbraco.Application.Models
{
    public class AzureSearchScoringFunction
	{
		public AzureSearchScoringFunctionType Type { get; set; }
		public string FieldName { get; set; }
		public int Boost { get; set; }
		public Nullable<ScoringFunctionInterpolation> Interpolation { get; set; }
        public string Params { get; set; }

		public ScoringFunction GetEffectiveScoringFunction()
		{
			switch (this.Type)
			{
				case AzureSearchScoringFunctionType.freshness:
                    var freshnessSettings = JsonConvert.DeserializeObject<Freshness>(this.Params);
					var fressnessParams = new FreshnessScoringParameters(TimeSpan.Parse(freshnessSettings.BoostingDuration));
					return new FreshnessScoringFunction(this.FieldName, this.Boost, fressnessParams, this.Interpolation);
				case AzureSearchScoringFunctionType.magnitude:
                    var magnitudeSettings = JsonConvert.DeserializeObject<Magnitude>(this.Params);
                    var magnitudeParams = new MagnitudeScoringParameters(magnitudeSettings.BoostingRangeStart, magnitudeSettings.BoostingRangeEnd, magnitudeSettings.ConstantBoostBeyondRange);
					return new MagnitudeScoringFunction(this.FieldName, this.Boost, magnitudeParams, this.Interpolation);
				case AzureSearchScoringFunctionType.distance:
                    var distanceSettings = JsonConvert.DeserializeObject<Distance>(this.Params);
                    var distanceParams = new DistanceScoringParameters(distanceSettings.ReferencePointParameter, distanceSettings.BoostingDistance);
					return new DistanceScoringFunction(this.FieldName, this.Boost, distanceParams, this.Interpolation);
				case AzureSearchScoringFunctionType.tag:
                    var tagSettings = JsonConvert.DeserializeObject<Tag>(this.Params);
                    var tagParams = new TagScoringParameters(tagSettings.TagsParameter);
					return new TagScoringFunction(this.FieldName, this.Boost, tagParams, this.Interpolation);
				default:
					throw new NotSupportedException($"{this.Type}");
			}
		}
	}

	#region Parameter Mapping Models

	public class Freshness
	{
		public string BoostingDuration { get; set; }

		/// <summary>
		/// TODO: This should be specified and converted from XSD "dayTimeDuration" format.
		/// https://docs.microsoft.com/en-us/rest/api/searchservice/add-scoring-profiles-to-a-search-index#bkmk_boostdur
		/// For now - parse as days.
		/// </summary>
		public TimeSpan BoostingDurationParsed
		{
			get
			{
				return TimeSpan.FromDays(Int32.Parse(this.BoostingDuration));
			}
		}
	}

	public class Magnitude
	{
		public double BoostingRangeStart { get; set; }
		public double BoostingRangeEnd { get; set; }
		public Nullable<bool> ConstantBoostBeyondRange { get; set; }
	}

	public class Distance
	{
		public string ReferencePointParameter { get; set; }
		public double BoostingDistance { get; set; }
	}

	public class Tag
	{
		public string TagsParameter { get; set; }
	}

	#endregion
}
