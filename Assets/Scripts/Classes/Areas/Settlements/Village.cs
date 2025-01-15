using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Village : Settlement
{
    private const int CORNER_SHOP_SPAWN_RATE = 40;

    public Village()
    {
        name = "Village";
    }

    /// <summary>
    /// Returns all Areas connected to this area.
    /// </summary>
    /// <returns>A list of Areas connected to this area.</returns>
    public override List<Area> GetConnections()
    {
        return new List<Area>();
    }

    /// <summary>
    /// Generates North and South districts for a Village.
    /// Both districts have 1 Residential Estate each and at least 1 Corner Shop between them.
    /// </summary>
    /// <returns>The North & South districts.</returns>
    protected override List<District> GenerateDistricts()
    {
        List<District> d = new List<District>();
        bool cornerShopGen = false;
        // Generate North district
        List<Area> north = new List<Area>(){new ResidentialEstate(false)};
        if (Helper.ChanceEvent(new List<int>() { CORNER_SHOP_SPAWN_RATE }))
        {
            north.Add(new CornerShop());
            cornerShopGen = true;
        }
        d.Add(new District("North", north));
        // Generate South district
        List<Area>  south = new List<Area>() { new ResidentialEstate(false) };
        if (!cornerShopGen)
        {
            south.Add(new CornerShop());
        }
        else if (Helper.ChanceEvent(new List<int>() { CORNER_SHOP_SPAWN_RATE })) south.Add(new CornerShop());
        d.Add(new District("South", south));
        return d;
    }

}
