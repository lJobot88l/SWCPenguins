namespace SWCPenguins
{
	class GameSession
	{
		public static GameSession currentsession;
		public string roomid;
		public string colour;
		public GameSession(string rId, string color)
		{
			colour = color;
			roomid = rId;
		}
	}
}