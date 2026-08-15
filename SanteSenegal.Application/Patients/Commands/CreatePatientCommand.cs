using MediatR;

namespace SanteSenegal.Application.Patients.Commands;

public record CreatePatientCommand(string Nom, string Prenom, DateTime DateNaissance, string Telephone) : IRequest<int>;
