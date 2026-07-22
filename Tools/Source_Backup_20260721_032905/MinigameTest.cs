using System;

[Serializable]
public class MinigameTest
{
	public MINIGAME_TYPE m_test_type;

	public MinigameWall m_test_wall = new MinigameWall();

	public MinigamePassing m_test_passing = new MinigamePassing();

	public MinigameShooting m_test_shooting = new MinigameShooting();

	public MinigameChallenge CurrentTest => m_test_type switch
	{
		MINIGAME_TYPE.WALL => m_test_wall, 
		MINIGAME_TYPE.PASSING => m_test_passing, 
		MINIGAME_TYPE.SHOOTING => m_test_shooting, 
		_ => m_test_wall, 
	};

	public void OnDrawGizmos(Minigame minigame)
	{
		m_test_wall.OnDrawGizmos(minigame);
		m_test_passing.OnDrawGizmos(minigame);
		m_test_shooting.OnDrawGizmos(minigame);
	}
}
