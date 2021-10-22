using UnityEngine;
using UnityEngine.UI;
using MathNM;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class MapSetup : MonoBehaviour
{
    private Village _village;
    private Player _player;
    private RectTransform _playerDot;
    private RectTransform _playerDirection;



    [Zenject.Inject]
    public void Construct(Village village, DayNightCycle dayCycle, Player player)
    {
        _village = village;
        dayCycle.UpdateValuesEvent.AddListener(UpdateMap);
        _player = player;
    }


    private RectTransform[] _houseBoxes;

    private float _mapSize = 200f;
    private float _villageSize = 50f;
    private float _mapSizeOffset = 20f;

    private Vector3 _centre;

    private float _shrinkRatio => _mapSize / _villageSize;

    private void Awake()
    {
        _playerDot = transform.Find("Player Total").Find("Player Map").As<RectTransform>();
        _playerDirection = _playerDot.GetChild(0).As<RectTransform>();
    }

    private void Start()
    {
        SetupHouses();

    }

    private void LateUpdate()
    {
        UpdatePlayer();
    }

    private void OnEnable()
    {
        if (_houseBoxes == null) return;
        UpdateMap();
    }

    private void UpdatePlayer()
    {
        _playerDot.anchoredPosition = SetPositionOnMap(_player.Position);
        _playerDirection.localEulerAngles = new Vector3(0f, 0f, -_player.FacingAt.AngleOnXZdeg());
    }

    private Vector2 SetPositionOnMap(Vector3 posInRealWorld)
    {
        return _shrinkRatio * (posInRealWorld - _centre).toVector2();
    }

    private void SetupHouses()
    {
        CalcCenter();
        CalculateShringRatio();
        CreateHouseBoxes();
        SetPositionsAndSizes();
    }

    public void UpdateMap()
    {
        UpdateNumbers();
        labelInfected();
    }


    private void UpdateNumbers()
    {
        foreach (var building in _village.Zip(_houseBoxes, (r, g) => new { real = r, gui = g }))
        {
            TextMeshProUGUI[] textGui = building.gui.parent.GetComponentsInChildren<TextMeshProUGUI>();
            textGui[0].text = building.real.TotalResidents.ToString();
            int infected = building.real.ResidentsInfected;
            if (infected == 0)
            {
                textGui[1].enabled = false;
            }
            else
            {
                textGui[1].enabled = true;
                textGui[1].text = infected.ToString();
            }
        }
    }

    private void labelInfected()
    {
        foreach (var building in _village.Zip(_houseBoxes, (r, g) => new { real = r, gui = g }))
        {
            Image image = building.gui.GetComponent<Image>();
            if (building.real.IsInfected())
            {
                if (building.real.AppliedActions())
                {
                    image.color = new Color(1f, 0.5f, 0f);
                }
                else
                {
                    image.color = Color.red;
                }
            }
            else if (building.real.Dead)
            {
                image.color = Color.black;
            }
            else
            {
                image.color = new Color(0.5f, 0.3f, 0f);
            }
        }
    }

    private void CalcCenter()
    {
        _centre = Vector3.zero;
        foreach (Building building in _village)
        {
            _centre += building.position;
        }
        _centre /= _village.NumberOfBuildings;
    }

    private void CalculateShringRatio()
    {
        float dist = findMaxDistance();
        _villageSize = (dist + _mapSizeOffset) * 1.75f;
    }

    private float findMaxDistance()
    {
        float maxdistsq = 0f;
        foreach (Building building in _village)
        {
            float tempdistsq = (building.position - _centre).sqrMagnitude;
            if (tempdistsq > maxdistsq)
            {
                maxdistsq = tempdistsq;
            }
        }
        return maxdistsq.Sqrt();
    }

    private void CreateHouseBoxes()
    {
        _houseBoxes = new RectTransform[_village.NumberOfBuildings];
        _houseBoxes[0] = transform.GetChild(0).GetChild(0).GetChild(0).As<RectTransform>();
        for(int i = 1; i < _houseBoxes.Length; i++)
        {
            _houseBoxes[i] = Instantiate(_houseBoxes[0].parent, _houseBoxes[0].parent.parent).GetChild(0).As<RectTransform>();
        }
    }

    private void SetPositionsAndSizes()
    {
        var buildings = _village.Zip(_houseBoxes, (r, g) => new { real = r, gui = g });
        foreach (var building in buildings)
        {
            building.gui.parent.As<RectTransform>().anchoredPosition = SetPositionOnMap(building.real.position);
            building.gui.sizeDelta = _shrinkRatio * building.real.SizeAsSeenFromAbove;
            building.gui.rotation = Quaternion.Euler(0f, 0f, -building.real.transform.eulerAngles.y);
        }


    }
}
