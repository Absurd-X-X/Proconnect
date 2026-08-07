using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.AddCertificate;
using static Application.Commands.AddEducation;
using static Application.Commands.AddExperience;
using static Application.Commands.AddProfessionalSkill;
using static Application.Commands.AddProject;
using static Application.Commands.UpdateProfessionalProfile;

namespace Host.Controllers.Professional
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalController(IMediator mediator) : ControllerBase
    {
        [HttpPost("update-professional-profile")]

        public async Task<IActionResult> UpdateProfile(UpdateProfessionalProfileCommand profile)
        {
            var response = await mediator.Send(profile);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-experience")]
        public async Task<IActionResult> AddExperience(AddExperienceCommand experience)
        {
            var response = await mediator.Send(experience);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-education")]
        public async Task<IActionResult> AddEducation(AddEducationCommand education)
        {
            var response = await mediator.Send(education);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-certificate")]
        public async Task<IActionResult> AddCertificate(AddCertificateCommand certificate)
        {
            var response = await mediator.Send(certificate);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-professional-skills")]
        public async Task<IActionResult> AddSkills(AddProfessionalSkillCommand skills)
        {
            var response = await mediator.Send(skills);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-project")]

        public async Task<IActionResult> AddProject(AddProjectCommand project)
        {
            var response = await mediator.Send(project);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
