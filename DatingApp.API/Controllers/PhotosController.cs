using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user/{userid}/photos")]
    public class PhotosController : ControllerBase
    {
        public IDatingRepository _repo;
        public IMapper _mapper ;

        public IOptions<CloudinarySetting> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repository, IMapper mapper, IOptions<CloudinarySetting> cloudinaryConfig)
        {
            _repo = repository;
            _mapper = mapper;
            _cloudinaryConfig= cloudinaryConfig;


            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName , 
                _cloudinaryConfig.Value.ApiKey,
                 _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name="GetPhotoRoute")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForuser(int userId, [FromForm]PhotoForCreationDto photoForCreation)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);
            var file = photoForCreation.File;
            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreation.Url = uploadResult.Uri.ToString();
            photoForCreation.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreation);
            if(userFromRepo.Photos.Any(u => u.isMain == true)){
                    photo.isMain = false;
            } 
            else {
                photo.isMain = true;
            }
            
            userFromRepo.Photos.Add(photo);
            
            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhotoRoute",new {id = photo.Id, userId = userId},photoToReturn);
            }

            return BadRequest("Could not upload the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.isMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            if(currentMainPhoto != null) {
                currentMainPhoto.isMain = false;
            }
            photoFromRepo.isMain = true;

            if(await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("Could not set photo to main");
        }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
         if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }
          var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.isMain)
                return BadRequest("Cannot delete main photo");

        if (photoFromRepo.PublicId != null)
        {
            var deleteParams = new DeletionParams(photoFromRepo.PublicId);
            var results = _cloudinary.Destroy(deleteParams);
            if (results.Result.Equals("ok"))
                _repo.Delete(photoFromRepo);
        }

        if(photoFromRepo.PublicId == null) 
            _repo.Delete(photoFromRepo);

        
        if (await _repo.SaveAll())
            return Ok();

        return BadRequest("Failed to delete the photo");

    }
    }
}