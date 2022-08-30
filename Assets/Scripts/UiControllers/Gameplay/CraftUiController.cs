using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Gameplay;
using RPGGame.Gameplay.Events;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UiControllers
{
    public class CraftUiController : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private Transform _recipesList;
        [SerializeField] private RecipeSlotUiController _recipePrefab;
        [SerializeField] private List<RecipeSlotUiController> _recipes;

        [SerializeField] private Transform _requiredList;
        [SerializeField] private InventorySlotUiController _requiredPrefab;
        [SerializeField] private List<InventorySlotUiController> _requires;

        [SerializeField] private Button _craftButton;
        [SerializeField] private Slider _craftProgress;

        [SerializeField] private WorkbenchesConfig _workbenchesConfig;
        [SerializeField] private GameplayEvents _gameplayEvents;

        private Inventory _inventory;
        private Crafter _crafter;
        private CraftRecipeConfig _selectedRecipe;

        public event Action Closed;

        private void Awake()
        {
            _closeButton.onClick.AddListener(OnClose);
            _craftButton.onClick.AddListener(OnCraft);
        }

        private void OnEnable()
        {
            _gameplayEvents.OnInventoryChanged += OnInventoryChanged;
            _gameplayEvents.OnCraftProgressChanged += OnCraftProgressChanged;
        }

        private void OnDisable()
        {
            _gameplayEvents.OnInventoryChanged -= OnInventoryChanged;
            _gameplayEvents.OnCraftProgressChanged -= OnCraftProgressChanged;
        }

        public void Initialize(NetworkString<_32> workbenchId)
        {
            _inventory = PlayerCharacter.Local.GetComponent<Inventory>();
            _crafter = PlayerCharacter.Local.GetComponent<Crafter>();

            _recipes.Clear();
            foreach (Transform child in _recipesList.transform)
                Destroy(child.gameObject);

            WorkbenchConfig workbenchConfig = _workbenchesConfig.GetItem(workbenchId.Value);
            foreach(var recipeConfig in workbenchConfig.Recipes)
            {
                RecipeSlotUiController recipeSlot = Instantiate(_recipePrefab, _recipesList);
                _recipes.Add(recipeSlot);
                recipeSlot.Initialize(recipeConfig);
                recipeSlot.OnSelect += OnSelectRecipe;
            }

            OnSelectRecipe(workbenchConfig.Recipes[0]);

            _craftProgress.gameObject.SetActive(false);
        }

        private void OnSelectRecipe(CraftRecipeConfig recipeConfig)
        {
            Debug.Log($"Selected {recipeConfig.Result.Config.Name}");

            foreach (RecipeSlotUiController recipeSlot in _recipes)
                recipeSlot.SetSelected(false);
            _recipes.Find(slot => slot.Config == recipeConfig).SetSelected(true);

            _requires.Clear();
            foreach (Transform child in _requiredList.transform)
                Destroy(child.gameObject);

            foreach(var requiredItem in recipeConfig.Requires)
            {
                InventorySlotUiController itemSlot = Instantiate(_requiredPrefab, _requiredList);
                _requires.Add(itemSlot);
                itemSlot.Initialize(requiredItem.Config);
            }

            UpdateRequired(recipeConfig);

            _selectedRecipe = recipeConfig;
        }

        private void UpdateRequired(CraftRecipeConfig recipeConfig)
        {
            bool isAvalable = true;

            foreach(var itemSlot in _requires)
            {
                _inventory.Items.TryGet(itemSlot.ItemConfig.ID, out int available);
                CraftRecipeConfig.RecipeItem recipeItem = recipeConfig.Requires.Find(item => item.Config == itemSlot.ItemConfig);
                isAvalable = available >= recipeItem.Count;
                itemSlot.Initialize(itemSlot.ItemConfig, $"{available}/{recipeItem.Count}", available >= recipeItem.Count ? Color.green : Color.red);

                if (recipeItem.Count > available)
                    isAvalable = false;
            }

            _craftButton.interactable = isAvalable;
        }

        private void OnClose()
        {
            PlayerCharacter.Local.GetComponent<Interactor>().InterruptInteract();
        }

        private void OnCraft()
        {
            _crafter.CraftRecipe(_selectedRecipe.ID);
        }

        private void OnInventoryChanged(NetworkDictionary<NetworkString<_32>, int> inventory)
        {
            UpdateRequired(_selectedRecipe);
        }

        private void OnCraftProgressChanged(float progress)
        {
            _craftProgress.gameObject.SetActive(progress > 0f);
            _craftProgress.value = progress;
        }
    }
}
