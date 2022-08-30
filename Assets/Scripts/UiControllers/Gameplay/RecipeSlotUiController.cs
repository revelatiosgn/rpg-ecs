using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Gameplay;
using RPGGame.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UiControllers
{
    public class RecipeSlotUiController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _selected;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _count;

        private CraftRecipeConfig _config;
        public CraftRecipeConfig Config => _config;

        public event Action<CraftRecipeConfig> OnSelect;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnSelect?.Invoke(_config));
            _selected.gameObject.SetActive(false);
        }

        public void Initialize(CraftRecipeConfig config)
        {
            _config = config;
            _icon.sprite = config.Result.Config.Icon;
            _count.text = config.Result.Count.ToString();
        }

        public void SetSelected(bool value)
        {
            _selected.gameObject.SetActive(value);
        }
    }
}
