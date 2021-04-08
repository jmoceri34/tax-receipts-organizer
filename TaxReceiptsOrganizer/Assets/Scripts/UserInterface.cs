using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour 
{
	public UILabel receiptBundleLabel;
	public UIInput receiptBundleInput;
	public UIScrollView svReceiptBundle;
	public PlayerController pController;
	public GameObject createTaxBundleContainer, viewTaxBundleContainer, useWizardContainer, receiptManagerContainer,
	addCategoryWindow, addReceiptBundleWindow, createReceiptWindow;

	void Start () 
	{
		receiptBundleInput.onChange.Add(new EventDelegate(updateScroll));

	}
	
	public void updateScroll()
	{
		//Debug.Log("second");
		svReceiptBundle.UpdateScrollbars(true);
		//svReceiptBundle.UpdatePosition();
	}

	public void toggleReceiptManager()
	{
		playButtonSound();
		pController.saveReceiptBundle();
		bool rmActive = receiptManagerContainer.activeSelf, uwActive = useWizardContainer.activeSelf;

		receiptManagerContainer.SetActive(!rmActive);
		useWizardContainer.SetActive(!uwActive);

	}

	public void toggleViewTaxBundles()
	{
		playButtonSound();
		viewTaxBundleContainer.SetActive(!viewTaxBundleContainer.activeSelf);
	}

	public void toggleCreateTaxBundles()
	{
		playButtonSound();
		createTaxBundleContainer.SetActive(!createTaxBundleContainer.activeSelf);
	}

	public void closeTaxBundleWindow()
	{
		playButtonSound();
		viewTaxBundleContainer.SetActive(false);
	}

	public void toggleAddCategoryWindow()
	{
		playButtonSound();
		addCategoryWindow.SetActive(!addCategoryWindow.activeSelf);
	}

	public void toggleAddReceiptBundleWindow()
	{
		playButtonSound();
		addReceiptBundleWindow.SetActive(!addReceiptBundleWindow.activeSelf);
	}

	public void closeAddCategoryWindow()
	{
		playButtonSound();
		addCategoryWindow.SetActive(false);
	}

	public void closeAddReceiptBundleWindow()
	{
		playButtonSound();
		addReceiptBundleWindow.SetActive(false);
	}

	public void closeCreateReceiptWindow()
	{
		playButtonSound();
		createReceiptWindow.SetActive(false);
	}

	public void AddReceiptsToReceiptBundle()
	{
		pController.playAddSound();
		pController.AddToReceiptBundle();
	}

	public void saveReceiptBundle()
	{
		pController.playButtonSound();
		pController.saveReceiptBundle();
	}

	public void AddReceiptBundleToCategory() 
	{
		playButtonSound();

	}

	public void playButtonSound()
	{
		pController.buttonSound.Stop();
		pController.buttonSound.Play();
	}

}
