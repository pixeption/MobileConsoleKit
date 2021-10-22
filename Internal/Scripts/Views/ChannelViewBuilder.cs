using System.Collections.Generic;
using System.Linq;

namespace MobileConsole.UI
{
	public class ChannelViewBuilder : ViewBuilder
	{
		Node _channelCategoryNode;
		CheckboxNodeView _newChannelEnabledNode;
		CheckboxNodeView _nonChannelEnabledNode;
		bool _isDirty;

		public ChannelViewBuilder()
		{
			CategoryNodeView quickSettingsNode = CreateCategories("Quick Settings");
			_newChannelEnabledNode = AddCheckbox("Enable New Channel By Default", LogFilter.Instance.IsChannelEnabledByDefault(), OnChannelEnableByDefaultChanged, quickSettingsNode);
			_nonChannelEnabledNode = AddCheckbox("Show Non-Channel Logs", LogFilter.Instance.IsShowNonChannel(), OnShowNonChannelChanged, quickSettingsNode);
			AddButton("Enable All", "action", EnableAll, quickSettingsNode);
			AddButton("Disable All", "action", DisableAll, quickSettingsNode);
			_channelCategoryNode = CreateCategories("Channels");
		}

		// This function is called before rebuild, thus no rebuild is needed to call here
		public override void OnPrepareToShow()
		{
			base.OnPrepareToShow();
			
			_isDirty = false;
			_channelCategoryNode.children.Clear();
			LogChannelInfoCache.OnNewChannelsAdded += OnNewChannelsAdded;

			List<KeyValuePair<int, string>> channels = LogChannelInfoCache.GetChannels().ToList();
			channels.Sort((c1, c2) => c1.Value.CompareTo(c2.Value));
			foreach (var channel in channels)
			{
				AddChannelNode(channel.Key, channel.Value);
			}
		}

		public override void OnPrepareToHide()
		{
			base.OnPrepareToHide();
			LogChannelInfoCache.OnNewChannelsAdded -= OnNewChannelsAdded;
		}

		void OnNewChannelsAdded(int[] channelHashes)
		{
			foreach (var hash in channelHashes)
			{
				AddChannelNode(hash, LogChannelInfoCache.GetChannelName(hash));
			}
			
			Rebuild();
		}

		void OnChannelEnableByDefaultChanged(CheckboxNodeView nodeView)
		{
			LogFilter.Instance.SetChannelEnabledByDefault(nodeView.isOn);
		}

		void AddChannelNode(int hash, string name)
		{
			bool enable = LogFilter.Instance.IsChannelEnabled(hash);
			string colorName = TextColors.GetUniqueColorWithTag(name);
			Node node = AddCheckbox(colorName, enable, OnChannelEnableChanged, _channelCategoryNode);
			node.data = hash;
		}

		void EnableAll(GenericNodeView nodeView)
		{
			SetEnableAll(true);
		}

		void DisableAll(GenericNodeView nodeView)
		{
			SetEnableAll(false);
		}

		void SetEnableAll(bool isEnable)
		{
			_isDirty = true;

			LogFilter.Instance.SetAllChannelEnable(isEnable);
			foreach (CheckboxNodeView nodeView in _channelCategoryNode.children)
			{
				nodeView.isOn = isEnable;
			}
			
			
			LogFilter.Instance.SetChannelEnabledByDefault(isEnable);
			LogFilter.Instance.SetShowNonChannel(isEnable);
			_newChannelEnabledNode.isOn = isEnable;
			_nonChannelEnabledNode.isOn = isEnable;
			
			RefreshUI();
		}

		void OnChannelEnableChanged(CheckboxNodeView nodeView)
		{
			int hash = (int)nodeView.data;
			LogFilter.Instance.SetChannelEnabled(hash, nodeView.isOn);
			_isDirty = true;
		}

		void OnShowNonChannelChanged(CheckboxNodeView nodeView)
		{
			LogFilter.Instance.SetShowNonChannel(nodeView.isOn);
			_isDirty = true;
		}

		public bool IsFilterChanged()
		{
			return _isDirty;
		}
	}
}