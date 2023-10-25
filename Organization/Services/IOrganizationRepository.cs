namespace Organization.Services;

public interface IOrganizationRepository
{ 
    string? Create(Models.Organization organization);
}