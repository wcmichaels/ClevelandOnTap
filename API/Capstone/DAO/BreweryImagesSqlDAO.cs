﻿using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.DAO
{
    public class BreweryImagesSqlDAO : IBreweryImagesDAO
    {
        private readonly string connectionString;
        private const string SQL_GET_IMAGES = "SELECT * FROM brewery_images WHERE brewery_id = @breweryId;";
        private const string SQL_CREATE_IMAGE = "INSERT INTO brewery_images (brewery_id, image_url) VALUES (@breweryId, @imageUrl); SELECT * FROM brewery_images WHERE image_id = @@identity;";

        public BreweryImagesSqlDAO(string dbConnectionString)
        {
            this.connectionString = dbConnectionString;
        }

        public List<BreweryImage> GetImages(int breweryId)
        {
            List<BreweryImage> images = new List<BreweryImage>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_IMAGES, conn);
                    cmd.Parameters.AddWithValue("@breweryId", breweryId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        BreweryImage image = RowToObject(reader);
                        images.Add(image);
                    }
                }
                return images;
            }
            catch (SqlException ex)
            {

                throw;
            }
        }


        public BreweryImage CreateImage(BreweryImage brewery)
        {
            BreweryImage bi = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_CREATE_IMAGE, conn);
                    cmd.Parameters.AddWithValue("@breweryId", brewery.BreweryId);
                    cmd.Parameters.AddWithValue("@imageUrl", brewery.ImageUrl);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        bi = RowToObject(reader);
                    }
                }

                return bi;
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        private static BreweryImage RowToObject(SqlDataReader reader)
        {
            BreweryImage bi = new BreweryImage();

            bi.BreweryId = Convert.ToInt32(reader["brewery_id"]);
            bi.ImageUrl = Convert.ToString(reader["image_url"]);
            bi.ImageId = Convert.ToInt32(reader["image_id"]);

            return bi;
        }


    }
}
