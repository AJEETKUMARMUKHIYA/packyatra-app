
    using MoversAndPackerApi.Models;
    using MoversAndPackerApi.Services.Interfaces;
   
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MoversAndPackerApi.Services
    {
        public class ConsignmentService : IConsignmentService
        {
            // TEMP: In-memory store (replace with DB / Dapper / EF later)
            private static readonly List<GoodsConsignmentNote> _consignments = new();

            public async Task<GoodsConsignmentNote> CreateAsync(GoodsConsignmentNote model)
            {
                // Simulate async DB call
                await Task.Delay(50);

                model.Id = _consignments.Count + 1;
                model.GcNumber ??= GenerateGcNumber();

                _consignments.Add(model);
                return model;
            }

            public async Task<GoodsConsignmentNote> GetByGcNumberAsync(string gcNumber)
            {
                await Task.Delay(30);

                return _consignments
                    .FirstOrDefault(x => x.GcNumber.Equals(gcNumber, StringComparison.OrdinalIgnoreCase));
            }

            public async Task<List<GoodsConsignmentNote>> GetAllAsync()
            {
                await Task.Delay(30);
                return _consignments.OrderByDescending(x => x.GeneratedDate).ToList();
            }

            public async Task<bool> DeleteAsync(int id)
            {
                await Task.Delay(30);

                var item = _consignments.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return false;

                _consignments.Remove(item);
                return true;
            }

            private string GenerateGcNumber()
            {
                return $"GC-{DateTime.UtcNow:yyyyMMddHHmmss}";
            }
        }
    }
