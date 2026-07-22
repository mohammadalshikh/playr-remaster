using System.Collections.Generic;
using UnityEngine;

public class RewardManager
{
	public enum THRESHOLD
	{
		TIER_1,
		TIER_2,
		TIER_3,
		TIER_4,
		TIER_5,
		TIER_6
	}

	private static int[] _scoreTier = new int[6] { 20000, 50000, 100000, 150000, 500000, 1000000 };

	private static Dictionary<string, int[]> _matchFinessRewards = new Dictionary<string, int[]>
	{
		{
			"countdown_wall",
			new int[6] { 4, 8, 12, 16, 20, 24 }
		},
		{
			"knockout_wall",
			new int[6] { 12, 15, 16, 20, 30, 30 }
		},
		{
			"lifeline_wall",
			new int[6] { 10, 12, 15, 18, 25, 30 }
		},
		{
			"rush_wall",
			new int[6] { 4, 10, 15, 20, 30, 30 }
		},
		{
			"countdown_pass",
			new int[6] { 4, 12, 20, 22, 25, 30 }
		},
		{
			"knockout_pass",
			new int[6] { 12, 15, 25, 30, 30, 30 }
		},
		{
			"lifeline_pass",
			new int[6] { 10, 15, 20, 25, 30, 30 }
		},
		{
			"rush_pass",
			new int[6] { 10, 12, 15, 20, 25, 30 }
		},
		{
			"countdown_shoot",
			new int[6] { 4, 10, 20, 25, 25, 25 }
		},
		{
			"lifeline_shoot",
			new int[6] { 10, 16, 20, 25, 30, 30 }
		},
		{
			"knockout_shoot",
			new int[6] { 12, 15, 20, 25, 30, 30 }
		},
		{
			"rush_shoot",
			new int[6] { 6, 10, 15, 25, 30, 30 }
		}
	};

	private static Dictionary<string, int[]> _coinRewards = new Dictionary<string, int[]>
	{
		{
			"countdown_wall",
			new int[6] { 2, 4, 6, 8, 26, 52 }
		},
		{
			"knockout_wall",
			new int[6] { 2, 10, 30, 40, 200, 200 }
		},
		{
			"lifeline_wall",
			new int[6] { 5, 10, 15, 30, 80, 160 }
		},
		{
			"rush_wall",
			new int[6] { 4, 6, 8, 10, 30, 80 }
		},
		{
			"countdown_pass",
			new int[6] { 2, 4, 6, 8, 26, 52 }
		},
		{
			"knockout_pass",
			new int[6] { 5, 10, 15, 30, 80, 80 }
		},
		{
			"lifeline_pass",
			new int[6] { 5, 10, 15, 30, 80, 80 }
		},
		{
			"rush_pass",
			new int[6] { 5, 10, 15, 20, 30, 80 }
		},
		{
			"countdown_shoot",
			new int[6] { 3, 6, 8, 15, 26, 52 }
		},
		{
			"lifeline_shoot",
			new int[6] { 4, 8, 12, 20, 30, 52 }
		},
		{
			"knockout_shoot",
			new int[6] { 2, 10, 30, 50, 500, 500 }
		},
		{
			"rush_shoot",
			new int[6] { 5, 10, 12, 18, 26, 52 }
		}
	};

	private static THRESHOLD _getTierForScore(int score)
	{
		if (score < _scoreTier[0])
		{
			return THRESHOLD.TIER_1;
		}
		if (score >= _scoreTier[0] && score < _scoreTier[1])
		{
			return THRESHOLD.TIER_2;
		}
		if (score >= _scoreTier[1] && score < _scoreTier[2])
		{
			return THRESHOLD.TIER_3;
		}
		if (score >= _scoreTier[2] && score < _scoreTier[3])
		{
			return THRESHOLD.TIER_4;
		}
		if (score >= _scoreTier[3] && score < _scoreTier[4])
		{
			return THRESHOLD.TIER_5;
		}
		if (score >= _scoreTier[4])
		{
			return THRESHOLD.TIER_6;
		}
		return THRESHOLD.TIER_1;
	}

	public static int MatchFitnessReward(string name, int score)
	{
		if (_matchFinessRewards.ContainsKey(name))
		{
			int num = 0;
			int num2 = (int)_getTierForScore(score);
			int num3;
			int num4;
			if (_getTierForScore(score) == THRESHOLD.TIER_1)
			{
				num3 = 0;
				num4 = _matchFinessRewards[name][num2];
				return Mathf.CeilToInt(Mathf.Lerp(num3, num4, Mathf.InverseLerp(0f, _scoreTier[num2], score)));
			}
			num = num2 - 1;
			num3 = _matchFinessRewards[name][num];
			num4 = _matchFinessRewards[name][num2];
			return Mathf.CeilToInt(Mathf.Lerp(num3, num4, Mathf.InverseLerp(_scoreTier[num], _scoreTier[num2], score)));
		}
		return 0;
	}

	public static int CoinReward(string name, int score)
	{
		if (_coinRewards.ContainsKey(name))
		{
			int num = 0;
			int num2 = (int)_getTierForScore(score);
			int num3;
			int num4;
			if (_getTierForScore(score) == THRESHOLD.TIER_1)
			{
				num3 = 0;
				num4 = _coinRewards[name][num2];
				return Mathf.CeilToInt(Mathf.Lerp(num3, num4, Mathf.InverseLerp(0f, _scoreTier[num2], score)));
			}
			num = num2 - 1;
			num3 = _coinRewards[name][num];
			num4 = _coinRewards[name][num2];
			return Mathf.CeilToInt(Mathf.Lerp(num3, num4, Mathf.InverseLerp(_scoreTier[num], _scoreTier[num2], score)));
		}
		return 0;
	}

	public static int HighestMatchAvailableForLevel()
	{
		return _matchFinessRewards[GlobalSettings.GetCurrentGame().Name][5];
	}
}
