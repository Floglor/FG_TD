using System;
using System.Linq;
using System.Text.RegularExpressions;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class LinkText : MonoBehaviour, IPointerClickHandler
{
    private Camera _camera;
    private TextMeshProUGUI textMeshProUGUI;
    private void Start()
    {
        FindAndColorKeys();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _camera = Camera.main;
    }

    public void FindAndColorKeys()
    {
        DescriptionLinks descriptionLinks = DescriptionLinks.Instance;
        
        TextMeshProUGUI uguiText = GetComponent<TextMeshProUGUI>();
        string text = uguiText.text;
        
        Regex regex = new Regex(@"##(\w*):[1-9]\d*(\.\d+)?");
        
        MatchCollection matches = regex.Matches(text);
        
        
        foreach (Match match in matches)
        {
            Regex keyRegex = new Regex(@"##(\w*)");
            Match keyMatch = keyRegex.Match(text);

            string key = keyMatch.Value;
            key = key.Replace("##", "");
            

            foreach (DescriptionLink descriptionLink in descriptionLinks.descriptionLinks.Where(descriptionLink => key.Equals(descriptionLink.key)))
            {
                Regex digitRegex = new Regex(@"[1-9]\d*(\.\d+)?");
                //(\d*)\.?(\d*) appears cursed????
                Match digitMatch = digitRegex.Match(text);

                MatchCollection matchCollection = digitRegex.Matches(text);
                
                uguiText.text = uguiText.text.Replace(match.Value, $"<link={descriptionLink.key}><color={descriptionLink.color}>{digitMatch}</color></link>");
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        DescriptionLinks descriptionLinks = DescriptionLinks.Instance;


        //Debug.Log(textMeshProUGUI.text);

        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshProUGUI, Input.mousePosition, _camera);
        
        
        if (linkIndex <= -1) return;
        
        TMP_LinkInfo linkInfo = textMeshProUGUI.textInfo.linkInfo[linkIndex];
        string linkId = linkInfo.GetLinkID();

        foreach (DescriptionLink descriptionLink in descriptionLinks.descriptionLinks.Where(descriptionLink => descriptionLink.key.Equals(linkId)))
        {
            ToolTipText.instance.ShowUp();
            ToolTipText.instance.ChangeText(descriptionLink.description);
        }   
    }
}
