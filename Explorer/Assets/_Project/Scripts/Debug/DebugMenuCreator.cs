using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class DebugMenuCreator : MonoBehaviour
{
    [SerializeField] private Transform _menuPanel;
    [SerializeField] private Transform _tabPanel;
    [SerializeField] private GameObject _menuPrefab;
    [SerializeField] private GameObject _subMenuPrefab;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _tabPrefab;

    [SerializeField] private Color _color;
    [SerializeField] private string _menuName;
    [SerializeField] private string _subMenuName;
    [SerializeField] private string _itemName;


    private const string ItemsMenuName = "Items";
    private const string TitleName = "Title";


    [Button]
    public void AddToMenu()
    {
        var menuNameEditor = "[" + _menuName + "]";
        var subMenuNameEditor = "[" + _subMenuName + "]";
        var itemNameEditor = "[" + _itemName + "]";

        //Check if menu exists
        var numMenus = _menuPanel.childCount;
        var menu = _menuPanel.Find(menuNameEditor);
        if (numMenus == 0 || !menu)        //If not, create it AND create TAB
        {
            //Create Menu
            menu = Instantiate(_menuPrefab, _menuPanel).transform;
            menu.name = menuNameEditor;

            //Create Tab
            var tab = Instantiate(_tabPrefab, _tabPanel).transform;
            tab.name = "[" + _menuName + " Tab]";
            var tabTitle = tab.Find(TitleName);
            if (tabTitle)
            {
                var tabTitleText = tabTitle.GetComponentInChildren<Text>();
                if (tabTitleText)
                {
                    tabTitleText.text = _menuName;
                }
            }

            //Update Tab Button
            var button = tab.GetComponent<Button>();
            var buttonOnClick = button.onClick;
            buttonOnClick.RemoveAllListeners();
            buttonOnClick.AddListener(() => menu.gameObject.SetActive(true));

            //Update All Other Tabs
            for (int i = 0; i < numMenus + 1; i++)
            {
                var childMenu = _menuPanel.GetChild(i);
                if (childMenu != menu)
                {
                    //Update this Tab
                    button.onClick.AddListener(delegate { childMenu.gameObject.SetActive(false); });

                    //Update other Tab
                    var otherTab = _tabPanel.Find(childMenu.name.Substring(0, childMenu.name.Length - 1) + " Tab]");
                    if (otherTab)
                    {
                        var otherButton = otherTab.GetComponent<Button>();
                        buttonOnClick.AddListener(() => menu.gameObject.SetActive(false));
                    }
                }
            }
        }

        //Check if subMenu exists
        var numSubMenus = menu.childCount;
        var subMenu = menu.Find(subMenuNameEditor);
        if (numSubMenus == 0 || !subMenu)        //If not, Create it
        {
            //Create SubMenu
            subMenu = Instantiate(_subMenuPrefab, menu).transform;
            subMenu.name = subMenuNameEditor;
        }

        var subTitle = subMenu.Find(TitleName);
        if (subTitle)
        {
            var subTitleText = subTitle.GetComponentInChildren<Text>();
            if (subTitleText)
                subTitleText.text = _subMenuName;
        }

        //Get Items Menu
        var itemsMenu = subMenu.Find(ItemsMenuName);
        if (!itemsMenu)
        {
            Debug.LogWarning("'Items' child object is missing from SubMenuPrefab", this);
            return;
        }
        var numItems = itemsMenu.childCount;
        var item = itemsMenu.Find(itemNameEditor);
        if (numItems == 0 || !item)        //If not, create it
        {
            //Create Item
            item = Instantiate(_itemPrefab, itemsMenu).transform;
            item.name = itemNameEditor;
        }

        //If it does exist, update it
        var itemButton = item.GetComponent<Button>();
        var onClick = itemButton.onClick;
        onClick.RemoveAllListeners();

        var itemText = item.GetComponentInChildren<Text>();
        itemText.text = _itemName + ": ";

        var image = item.GetComponentInChildren<Image>();
        if (image)
        {
            image.color = _color;
        }
    }
}
