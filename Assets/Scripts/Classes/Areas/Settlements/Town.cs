using System.Collections;
using System.Collections.Generic;

public class Town : Settlement
{
    public Town()
    {
        name = "Town";
    }

    #region Methods
    /// <summary>
    /// Returns all Areas connected to this area.
    /// </summary>
    /// <returns>A list of Areas connected to this area.</returns>
    public override List<Area> GetConnections()
    {
        return new List<Area>();
    }

    #region Generate districts methods
    /// <summary>
    /// Generates all four districts (North, East, South & West) for a Town.
    /// A Town must contain exactly 1 district with a Corner Shop & Visitor Centre.
    /// A Town cannot contain more than 1 Superstore.
    /// A district can contain either:
    ///     - Residential Estate only
    ///     - Residential Estate & Corner Shop
    ///     - Corner Shop & Visitor Centre
    ///     - Superstore only
    /// </summary>
    /// <returns>All four districts.</returns>
    protected override List<District> GenerateDistricts()
    {
        List<District> d = new List<District>();
        List<int> shelterCombos = new List<int>() { 1, 2, 3 }; //4
        // Generate North district
        List<Area> north = SelectShelterCombo(shelterCombos[Helper.random.Next(shelterCombos.Count)]);
        //if (DetectVisitorCentre(north)) shelterCombos.Remove(3);
        if (DetectSuperstore(north)) shelterCombos.Remove(4);
        d.Add(new District("North", north));
        // Generate East district
        List<Area> east = SelectShelterCombo(shelterCombos[Helper.random.Next(shelterCombos.Count)]);
        //if (DetectVisitorCentre(east)) shelterCombos.Remove(3);
        if (DetectSuperstore(east)) shelterCombos.Remove(4);
        d.Add(new District("East", east));
        // Generate South district
        List<Area> south = SelectShelterCombo(shelterCombos[Helper.random.Next(shelterCombos.Count)]);
        //if (DetectVisitorCentre(south)) shelterCombos.Remove(3);
        if (DetectSuperstore(south)) shelterCombos.Remove(4);
        d.Add(new District("South", south));
        // Generate West district
        /*
        if (shelterCombos.Contains(3)) // Check if a Visitor Centre has already been generated, if not then add one.
        {
            d.Add(new District("West", SelectShelterCombo(3)));
        }
        else { d.Add(new District("West", SelectShelterCombo(shelterCombos[Helper.random.Next(shelterCombos.Count)]))); }
        */
        d.Add(new District("West", SelectShelterCombo(shelterCombos[Helper.random.Next(shelterCombos.Count)])));
        return d;
    }

    /// <summary>
    /// Returns a list containing a specific combination of Shelters.
    /// </summary>
    /// <param name="randomInt">A randomly chosen integer that corresponds to a specfic combo.</param>
    /// <returns>The list of Shelters.</returns>
    private List<Area> SelectShelterCombo(int randomInt)
    {
        List<Area> s = new List<Area>();
        switch (randomInt)
        {
            case 1:
                s.Add(new ResidentialEstate(true));
                break;
            case 2:
                s.Add(new ResidentialEstate(true));
                s.Add(new CornerShop());
                break;
            case 3:
                s.Add(new Superstore());
                break;
            /*
            case 4:
                s.Add(new CornerShop());
                s.Add(new VisitorCentre());
                break;
            */
        }
        return s;
    }

    /// <summary>
    /// Checks if a list of Shelters contains a Superstore.
    /// </summary>
    /// <param name="shelters">The list of Shelters to check.</param>
    /// <returns>If a Superstore has been found.</returns>
    private bool DetectSuperstore(List<Area> shelters)
    {
        foreach (Area shelter in shelters)
        {
            if (shelter is Superstore) return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a list of Shelters contains a Visitor Centre.
    /// </summary>
    /// <param name="shelters">The list of Shelters to check.</param>
    /// <returns>If a Visitor Centre has been found.</returns>
    private bool DetectVisitorCentre(List<Area> shelters)
    {
        foreach (Area shelter in shelters)
        {
            if (shelter is VisitorCentre) return true;
        }
        return false;
    }
    #endregion
    #endregion
}
