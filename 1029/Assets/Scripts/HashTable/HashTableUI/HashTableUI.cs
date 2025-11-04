using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HashTableTestUI : MonoBehaviour
{
    private int currentSize = 16;

    enum Method
    {
        OpenAdressing,
        ChainingHash,
    }

    public TMP_Dropdown methodDropDown;
    public TMP_Dropdown probingDropDown;

    public TMP_InputField keyInputField;
    public TMP_InputField valueInputField;

    public TextMeshProUGUI hashHistory;

    public Button addButton;
    public Button removeButton;
    public Button clearButton;

    public ScrollRect tableViewRect;
    public GameObject emptySlot;
    public GameObject occupiedSlot;
    public GameObject indexBlock;

    private Method currentMethod = Method.OpenAdressing;

    private OpenAddressingHashTable<string, int> openHashTable;
    private ChainingHashTable<string, int> chainingHashTable;

    private string inputKey;
    private int inputValue;

    private bool isCleared;

    private RectTransform content;

    private List<GameObject> visualObjs;
    private bool[] usedChainingTable;
    

    
    private void Start()
    {
        content = tableViewRect.content;

        openHashTable = new OpenAddressingHashTable<string, int>();
        chainingHashTable = new ChainingHashTable<string, int>();

        visualObjs = new List<GameObject>();

        methodDropDown.onValueChanged.AddListener((i) => OnMethodValueChanged(i));
        probingDropDown.onValueChanged.AddListener((i) => OnProbingValueChanged(i));

        keyInputField.onValueChanged.AddListener(s => OnKeyFieldChanged(s));
        valueInputField.onValueChanged.AddListener(s => OnValueFieldChanged(s));

        addButton.onClick.AddListener(() => OnAddKVPClicked());
        removeButton.onClick.AddListener(() => OnRemoveKVPClicked());
        clearButton.onClick.AddListener(() => OnClearKVPClicked());

        isCleared = true;

        ResetVisualObjs();
        hashHistory.text = string.Empty;
    }

    private void SizeUpChainging(int inputIndex)
    {
        this.currentSize = chainingHashTable.Size;  // 현재 사이즈 갱신
        ResetVisualObjs();  // 오브젝트 리셋

        chainingHashTable.isSizeChanged = false;    // 사이즈 변경 플래그 초기화

        foreach (var key in chainingHashTable.Keys) 
        {
            int index = chainingHashTable.GetBucket(key);   // 버킷 인덱스 가져오기
            if (inputIndex == index)
                continue;

            if (!usedChainingTable[index])  // 해당 슬롯이 비어있다면
            {
                Destroy(visualObjs[index].transform.GetChild(0).gameObject);    // 비어있는 슬롯 오브젝트 제거
            }
            var obj = Instantiate(occupiedSlot, visualObjs[index].transform);   // 점유된 슬롯 오브젝트 생성
            SetSlotText(obj, index, key, chainingHashTable[key]);   // 슬롯 텍스트 설정

            usedChainingTable[index] = true;    // 해당 슬롯이 사용 중임을 표시
        }
    }
    private void SizeUpOpen(int inputIndex)
    {
        this.currentSize = openHashTable.Size;
        ResetVisualObjs();
        
        foreach (var key in openHashTable.Keys)
        {
            int index = openHashTable.FindIndex(key);
            if (inputIndex == index)
                continue;

            Destroy(visualObjs[index].transform.GetChild(0).gameObject);
            var obj = Instantiate(occupiedSlot, visualObjs[index].transform);
            SetSlotText(obj, index, key, openHashTable[key]);
        }
        openHashTable.isSizeChanged = false;
    }

    private void ResetVisualObjs()
    {
        visualObjs.Clear(); // 기존 오브젝트 리스트 초기화

        // 콘텐츠의 모든 자식 오브젝트 제거
        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i);
            Destroy(child.gameObject);
        }

        // 현재 크기만큼 인덱스 블록 생성 및 초기화
        for (int i = 0; i < currentSize; i++)
        {
            var obj = Instantiate(indexBlock, content);
            Instantiate(emptySlot, obj.transform);
            visualObjs.Add(obj);
            SetEmptyText(obj, i);
        }
        usedChainingTable = new bool[visualObjs.Count];
    }

    private void OnMethodValueChanged(int index)
    {
        if (!isCleared)
        {
            return;
        } 
        currentMethod = (Method)index;
    }

    private void OnProbingValueChanged(int index)
    {
        if (!isCleared)
        {
            return;
        } 

        openHashTable.ProbingStrategy = (ProbingStrategy)index;
    }

    private void OnKeyFieldChanged(string key)
    {
        inputKey = key;
    }

    private void OnValueFieldChanged(string value)
    {
        inputValue = int.Parse(value);
    }

    private void OnAddKVPClicked()
    {
        isCleared = false;

        var kvp = new KeyValuePair<string, int>(inputKey, inputValue);  // 추가할 키-값 쌍 생성

        // 현재 해시 테이블 방식에 따라 처리
        switch (currentMethod)
        {
            case Method.OpenAdressing:
                openHashTable.Add(kvp); // 키-값 쌍 추가

                // 크기가 변경된 경우
                if (openHashTable.isSizeChanged)
                {
                    SizeUpOpen(openHashTable.FindIndex(inputKey));
                }

                // 키가 존재하지 않는 경우
                if (openHashTable.FindIndex(inputKey) == -1)
                { 
                    return;
                }

                AddHistoryText($"ADD {inputKey}");  // 히스토리 텍스트 추가
                CheckUpdateSlot(kvp, occupiedSlot, false, openHashTable.FindIndex(inputKey));   // 슬롯 업데이트
                break;
            case Method.ChainingHash:
                // 체이닝 해시 테이블에 추가 및 슬롯 업데이트 처리
                CheckChainAddAndUpdateSlot(kvp);
                break;
        }
    }

    private void CheckUpdateSlot(KeyValuePair<string, int> kvp, GameObject slot, bool isRemove, int index)
    {
        if (index == -1)
        {
            return;
        }

        Destroy(visualObjs[index].transform.GetChild(0).gameObject);    // 기존 슬롯 오브젝트 제거
        var obj = Instantiate(slot, visualObjs[index].transform);   // 새로운 슬롯 오브젝트 생성
        if (!isRemove)
        {
            SetSlotText(obj, index, inputKey, inputValue);  // 슬롯 텍스트 설정
        }
        else
        {
            SetEmptyText(obj, index);   // 빈 슬롯 텍스트 설정
        }
    }

    // 체이닝 해시 테이블에 추가 및 슬롯 업데이트 처리
    private void CheckChainAddAndUpdateSlot(KeyValuePair<string, int> kvp)
    {
        var index2 = chainingHashTable.GetBucket(inputKey); // 버킷 인덱스 가져오기

        chainingHashTable.Add(kvp); // 키-값 쌍 추가

        // 크기가 변경된 경우
        if (chainingHashTable.isSizeChanged)
        {
            SizeUpChainging(index2);
        }

        // 빈 슬롯을 제거해야 하는지 여부 판단
        bool destroyEmpty = false;
        if (!usedChainingTable[index2])
        {
            destroyEmpty = true;
        }

        // 키가 이미 존재하는 경우
        if (chainingHashTable.ContainsKey(inputKey))
        {
            AddHistoryText($"ADD {inputKey}");  // 히스토리 텍스트 추가
            // 해당 버킷의 모든 자식 오브젝트 제거
            if (destroyEmpty)
            {
                Destroy(visualObjs[index2].transform.GetChild(0).gameObject);
            }

            // 해당 버킷의 모든 키-값 쌍에 대해 슬롯 오브젝트 생성
            var obj = Instantiate(occupiedSlot, visualObjs[index2].transform);
            SetSlotText(obj, index2, inputKey, inputValue);
            usedChainingTable[index2] = true;
        }
    }
    

    private void CheckChainRemoveAndUpdateSlot(KeyValuePair<string, int> kvp)
    {
        var index2 = chainingHashTable.GetBucket(inputKey); // 버킷 인덱스 가져오기

        // 키-값 쌍 제거 및 성공 여부 확인
        if (!chainingHashTable.Remove(kvp))
        {
            return;
        }

        AddHistoryText($"Remove {inputKey}");   // 히스토리 텍스트 추가
        var list = chainingHashTable.GetlistForKey(kvp.Key);    // 해당 키에 대한 리스트 가져오기

        if (list != null)   // 해당 버킷에 여전히 키-값 쌍이 남아있는 경우
        {
            // 해당 버킷의 모든 자식 오브젝트 제거
            for (int i = 0; i < visualObjs[index2].transform.childCount; i++)
            {
                Destroy(visualObjs[index2].transform.GetChild(i).gameObject);
            }

            // 남아있는 모든 키-값 쌍에 대해 슬롯 오브젝트 생성
            foreach (var ele in list)
            {
                var obj = Instantiate(occupiedSlot, visualObjs[index2].transform);
                SetSlotText(obj, index2, ele.Key, ele.Value);
            }
        }
        else    // 해당 버킷이 비어있는 경우
        {   
            Destroy(visualObjs[index2].transform.GetChild(0).gameObject);
            var obj = Instantiate(emptySlot, visualObjs[index2].transform);
            SetEmptyText(obj, index2);
            usedChainingTable[index2] = false;
        }
    }

    private void OnRemoveKVPClicked()
    {
        isCleared = false;  // 해시 테이블이 비워지지 않았음을 표시

        var kvp = new KeyValuePair<string, int>(inputKey, inputValue);  // 제거할 키-값 쌍 생성

        switch (currentMethod)
        {
            case Method.OpenAdressing:
                int index = openHashTable.FindIndex(kvp.Key);
                if (openHashTable.Remove(kvp))
                {
                    CheckUpdateSlot(kvp, emptySlot, true, index);
                    AddHistoryText($"Remove {inputKey}");
                }
                break;
            case Method.ChainingHash:
                CheckChainRemoveAndUpdateSlot(kvp);
                break;
        }
    }

    /// <summary>
    /// 해시 테이블과 시각적 오브젝트를 모두 초기화하는 메서드
    /// </summary>
    private void OnClearKVPClicked()
    {
        openHashTable.Clear();
        chainingHashTable.Clear();

        openHashTable = new OpenAddressingHashTable<string, int>();
        chainingHashTable = new ChainingHashTable<string, int>();

        currentSize = 16;
        ResetVisualObjs();
        isCleared = true;

        hashHistory.text = string.Empty;
    }

    private void AddHistoryText(string text)
    {
        var currStr = new StringBuilder(hashHistory.text);
        currStr.Append(text);
        currStr.Append(" -> ");

        hashHistory.text = currStr.ToString();
    }

    private void SetSlotText(GameObject obj, int index, string key, int value)
    {
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"I: {index} K: {key}\t V:{value}";
    }

    private void SetEmptyText(GameObject obj, int index)
    {
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"I: {index}";
    }
}