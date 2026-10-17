public class Health : global::UnityEngine.MonoBehaviour
{
	public ShipBase myShip;

	private global::UnityEngine.Events.UnityEvent dieAction = new global::UnityEngine.Events.UnityEvent();

	private global::UnityEngine.Events.UnityEvent afterHitAction = new global::UnityEngine.Events.UnityEvent();

	private global::UnityEngine.Events.UnityEvent healAction = new global::UnityEngine.Events.UnityEvent();

	private global::UnityEngine.Events.UnityEvent hpSetAction = new global::UnityEngine.Events.UnityEvent();

	[global::UnityEngine.SerializeField]
	private int healthPoint = 100;

	[global::UnityEngine.SerializeField]
	private int healthPointMax = 100;

	private int shieldEnergy;

	public float HpRatio => (float)healthPoint / (float)healthPointMax;

	public int HP => healthPoint;

	public int HP_MAX => healthPointMax;

	public int SHIELD_ENERGY => shieldEnergy;

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
	}

	public void SetHealthPoint(int health)
	{
		healthPoint = (healthPointMax = health);
		hpSetAction.Invoke();
	}

	public void SetFullHp()
	{
		healthPoint = HP_MAX;
		hpSetAction.Invoke();
	}

	public void SetHealthPoint(int health, int maxHealth)
	{
		healthPoint = health;
		healthPointMax = maxHealth;
		hpSetAction.Invoke();
	}

	public void AddShieldEnergy(int energyValue)
	{
		shieldEnergy += energyValue;
	}

	public void RemoveShield()
	{
		shieldEnergy = 0;
	}

	public void AddDieAction(global::UnityEngine.Events.UnityAction ac)
	{
		dieAction.RemoveListener(ac);
		dieAction.AddListener(ac);
	}

	public void AddHitAction(global::UnityEngine.Events.UnityAction ac)
	{
		afterHitAction.AddListener(ac);
	}

	public void RemoveHitAction(global::UnityEngine.Events.UnityAction ac)
	{
		afterHitAction.RemoveListener(ac);
	}

	public void AddHealAction(global::UnityEngine.Events.UnityAction ac)
	{
		healAction.AddListener(ac);
	}

	public void AddHpSetAction(global::UnityEngine.Events.UnityAction ac)
	{
		hpSetAction.AddListener(ac);
	}

	public void Hit(int damage)
	{
		if (shieldEnergy > 0)
		{
			shieldEnergy -= damage;
		}
		else
		{
			healthPoint -= damage;
			healthPoint = global::UnityEngine.Mathf.Max(0, healthPoint);
		}
		AudioManager.Instance.PlaySFX("ship_hit");
		if (healthPoint <= 0)
		{
			if (dieAction != null)
			{
				dieAction.Invoke();
			}
		}
		else if (afterHitAction != null)
		{
			afterHitAction.Invoke();
		}
	}

	public void Heal(int heal)
	{
		if (!(myShip != null) || !myShip.IsDie)
		{
			healthPoint += heal;
			healthPoint = global::UnityEngine.Mathf.Min(healthPoint, healthPointMax);
			if (healAction != null)
			{
				healAction.Invoke();
			}
		}
	}
}
