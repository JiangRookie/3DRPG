using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:4428a374-08c1-4802-9b79-96df550acf81
	public partial class MainMenuPanel
	{
		public const string Name = "MainMenuPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_NewGame;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Continue;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Quit;
		
		private MainMenuPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_NewGame = null;
			Btn_Continue = null;
			Btn_Quit = null;
			
			mData = null;
		}
		
		public MainMenuPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MainMenuPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MainMenuPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
