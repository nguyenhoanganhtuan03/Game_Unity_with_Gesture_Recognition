using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [SerializeField] Material[] bonusMaterials;
    [SerializeField] BonusType bonusType;

    private CollisionSoundEffect collisionSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        InitBonusType();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(80 * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (bonusType)
            {
                case BonusType.Coin:
                    ScoreManager.instance.AddPoints(50);
                    break;
                case BonusType.Emerald:
                    ScoreManager.instance.AddPoints(100);
                    break;
                case BonusType.Ruby:
                    ScoreManager.instance.AddPoints(200);
                    break;
                case BonusType.Diamond:
                    ScoreManager.instance.AddPoints(500);
                    break;
                case BonusType.DarkDiamond:
                    ScoreManager.instance.AddPoints(-500);
                    break;
            }
            collisionSoundEffect = other.GetComponent<CollisionSoundEffect>();
            collisionSoundEffect.PlayAndPause();
            Destroy(gameObject);
        }
    }

    private void InitBonusType()
    {
        var renderer = gameObject.GetComponent<Renderer>();
        int selectedBonus = GetRandomBonus();
        renderer.material = bonusMaterials[selectedBonus];

        switch (selectedBonus)
        {
            case 1:
                bonusType = BonusType.Coin;
                break;
            case 2:
                bonusType = BonusType.Emerald;
                break;
            case 3:
                bonusType = BonusType.Ruby;
                break;
            case 4:
                bonusType = BonusType.Diamond;
                break;
            case 5:
                bonusType = BonusType.DarkDiamond;
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
    }

    private int GetRandomBonus()
    {
        float rand = Random.value;

        int selectedBonus = 0;

        if (rand <= 0.8f)
        {
            selectedBonus = 1;
        }
        
        if (rand <= 0.14f)
        {
            selectedBonus = 2;
        } 
        
        if (rand <= 0.05f)
        {
            selectedBonus = 3;
        } 
        
        if (rand <= 0.005f)
        {
            selectedBonus = Mathf.RoundToInt(Random.Range(4.0f, 5.0f));
        }

        return selectedBonus;
    }
}

    public enum BonusType
    {
        Void,
        Coin,
        Emerald,
        Ruby,
        Diamond,
        DarkDiamond,
    }
