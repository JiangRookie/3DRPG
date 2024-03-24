// Generate Id:9b797bfb-84f3-4768-9d9b-3b23e3f79700
using UnityEngine;

namespace Jiang.Games
{
	public partial class MouseManager : QFramework.IController
	{

		public UnityEngine.Texture2D ArrowSprite;

		public UnityEngine.Texture2D TargetSprite;

		public UnityEngine.Texture2D AttackSprite;

		public UnityEngine.Texture2D DoorwaySprite;

		public UnityEngine.Texture2D PointSprite;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>Jiang.Games.Global.Interface;
	}
}
