using System;
using System.Linq;
using KitchenChaos.Interface;
using KitchenChaos.Player;
using KitchenChaos.SO;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class CuttingCounter : BaseCounter, IHasProgress
    {
        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSoArr;

        public event EventHandler<IHasProgress.OnProgressbarChangedEventArgs> OnProgressbarChanged;
        public event EventHandler OnCut;
        public static event EventHandler OnAnyCut;

        private CuttingRecipeSO _cuttingRecipeSo;
        private int _cuttingProgress = 0;

        public static new void ResetStaticData()
        {
            OnAnyCut = null;
        }

        public override void Interact(PlayerControl player)
        {
            if (!HasKitchenObject)
            {
                if (!player.HasKitchenObject)
                {
                }
                else
                {
                    if (HasRecipeWithInput(out CuttingRecipeSO cuttingRecipeSo, player.KitchenObj.KitchenObjectSo))
                    {
                        _cuttingRecipeSo = cuttingRecipeSo;
                        player.KitchenObj.SetKitchenObjectParent(this);
                        _cuttingProgress = 0;
                        OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                        {
                            progressNormalized = (float)_cuttingProgress / _cuttingRecipeSo.cuttingProgressMax
                        });
                    }
                }
            }
            else
            {
                if (!player.HasKitchenObject)
                {
                    KitchenObj.SetKitchenObjectParent(player);
                }
                else
                {
                    if (player.KitchenObj.TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                    {
                        plateKitchenObject = player.KitchenObj as PlateKitchenObject;
                        plateKitchenObject.AddIngredient(KitchenObj.KitchenObjectSo);
                        KitchenObj.DestroySelf();
                    }
                }
            }
        }

        public override void InteractAlternate(PlayerControl player)
        {
            if (HasKitchenObject && HasRecipeWithInput(out CuttingRecipeSO cuttingRecipeSo, KitchenObj.KitchenObjectSo))
            {
                _cuttingProgress++;
                OnProgressbarChanged?.Invoke(this, new IHasProgress.OnProgressbarChangedEventArgs
                {
                    progressNormalized = (float)_cuttingProgress / _cuttingRecipeSo.cuttingProgressMax
                });
                OnCut?.Invoke(this, EventArgs.Empty);
                OnAnyCut?.Invoke(this, EventArgs.Empty);
                if (_cuttingProgress < _cuttingRecipeSo.cuttingProgressMax) return;
                // var outputKitchenObjectSo = GetOutputByInput(KitchenObj.KitchenObjectSo);
                // Destroy
                KitchenObj.DestroySelf();
                // Instantiate
                KitchenObject.SpawnKitchenObject(cuttingRecipeSo.output, this);
            }
        }

        private CuttingRecipeSO GetCuttingRecipeSoByInput(KitchenObjectSO inputKitchenObjectSo)
        {
            return cuttingRecipeSoArr.FirstOrDefault(item => Equals(item.input, inputKitchenObjectSo));
        }

        private bool HasRecipeWithInput(out CuttingRecipeSO outCuttingRecipeSo, KitchenObjectSO inputKitchenObjectSo)
        {
            outCuttingRecipeSo = GetCuttingRecipeSoByInput(inputKitchenObjectSo);
            return outCuttingRecipeSo != null;
        }

        private KitchenObjectSO GetOutputByInput(KitchenObjectSO inputKitchenObjectSo)
        {
            var cuttingRecipeSo = GetCuttingRecipeSoByInput(inputKitchenObjectSo);
            return cuttingRecipeSo != null ? cuttingRecipeSo.output : null;
        }
    }
}