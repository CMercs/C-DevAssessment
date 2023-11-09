using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Developer_Assessment.Models;

namespace Developer_Assessment.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class NameAddressController : Controller
    {
        private readonly List<Address> addresses = new List<Address>();
        private int nextId = 1;

        /// <summary>
        /// Get a list of all addresses.
        /// </summary>
        /// <returns>List of addresses.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(addresses);
        }

        /// <summary>
        /// Get an address by Id.
        /// </summary>
        /// <param name="Id">Id associated with address to retrieve.</param>
        /// <returns>An Address associated with the ID passed.</returns>
        [HttpGet("{id}")]
        public IActionResult GetById()
        {
            return Ok(addresses);
        }

        /// <summary>
        /// Create a new address.
        /// </summary>
        /// <param name="addressInput">Address data.</param>
        /// <returns>The created address.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] AddressInputModel addressInput)
        {
            if (addressInput == null)
            {
                return BadRequest("Invalid data. Please provide a valid JSON request.");
            }

            if (string.IsNullOrWhiteSpace(addressInput.Name) || string.IsNullOrWhiteSpace(addressInput.Address))
            {
                return BadRequest("Name and Address fields are required.");
            }

            if (addresses.Any(a => a.Name.Equals(addressInput.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict($"Address with the name '{addressInput.Name}' already exists.");
            }

            // Create a new address object
            Address newAddress = new Address
            {
                Id = nextId,
                Name = addressInput.Name,
                AddressLine = addressInput.Address
            };

            addresses.Add(newAddress);

            nextId++;

            return Created($"/api/address/{newAddress.Id}", newAddress);
        }

        /// <summary>
        /// Update an existing address.
        /// </summary>
        /// <param name="id">Address ID.</param>
        /// <param name="addressInput">Updated address data.</param>
        /// <returns>The updated address.</returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AddressInputModel addressInput)
        {
            var existingAddress = addresses.FirstOrDefault(a => a.Id == id);
            if (existingAddress == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            if (string.IsNullOrWhiteSpace(addressInput.Name) || string.IsNullOrWhiteSpace(addressInput.Address))
            {
                return BadRequest("Name and Address fields are required.");
            }

            // Check for duplicate name
            if (addresses.Any(a => a.Name.Equals(addressInput.Name, StringComparison.OrdinalIgnoreCase) && a.Id != id))
            {
                return Conflict($"An address with the name '{addressInput.Name}' already exists.");
            }

            existingAddress.Name = addressInput.Name;
            existingAddress.AddressLine = addressInput.Address;

            return Ok(existingAddress);
        }

        /// <summary>
        /// Delete an address by ID.
        /// </summary>
        /// <param name="id">Address ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingAddress = addresses.FirstOrDefault(a => a.Id == id);
            if (existingAddress == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            addresses.Remove(existingAddress);

            return NoContent();
        }
    }
}
