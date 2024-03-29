﻿using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.DAO
{
    public class BrewerySqlDAO : IBreweryDAO
    {

        private readonly string connectionString;

        private const string SQL_GET_BREWERIES_AUTHENTICATED = @"SELECT b.*, bi.image_url, bu.user_id as favorite
                                                                     FROM breweries b
                                                                     LEFT OUTER JOIN (SELECT * FROM breweries_users WHERE user_id = @userId) bu ON bu.brewery_id = b.brewery_id
                                                                     JOIN(SELECT MIN(image_id) as min_id, brewery_id
                                                                      FROM brewery_images
                                                                      GROUP BY brewery_id) i ON i.brewery_id = b.brewery_id
                                                                      JOIN brewery_images bi ON bi.image_id = i.min_id";
        private const string SQL_GET_BREWERIES = @"SELECT b.*, bi.image_url
                                                    FROM breweries b
                                                    JOIN(SELECT MIN(image_id) as min_id, brewery_id
                                                        FROM brewery_images
                                                        GROUP BY brewery_id) i ON i.brewery_id = b.brewery_id
                                                        JOIN brewery_images bi ON bi.image_id = i.min_id;";

        private const string SQL_GET_BREWERY = "SELECT * FROM breweries WHERE brewery_id = @breweryId;";
        private const string SQL_GET_BREWERY_AUTHENTICATED = @"SELECT b.*
	                                                             FROM breweries b
	                                                             LEFT OUTER JOIN breweries_users bu ON bu.brewery_id = b.brewery_id
	                                                             WHERE b.brewery_id = @breweryId;
	                                                             SELECT bu.user_id as favorite
	                                                             FROM breweries b
	                                                             JOIN breweries_users bu ON bu.brewery_id = b.brewery_id
	                                                             WHERE b.brewery_id = @breweryId AND bu.user_id = @userId";
        private const string SQL_GET_BREWERIES_BY_BREWER = @"SELECT b.*, bi.image_url
	                                                        FROM breweries b
	                                                        JOIN (SELECT MIN(image_id) as min_id, brewery_id
			                                                    FROM brewery_images
			                                                    GROUP BY brewery_id) i ON i.brewery_id = b.brewery_id
			                                                    JOIN brewery_images bi ON bi.image_id = i.min_id
			                                                    WHERE b.user_id = @userId;";
        private const string SQL_CREATE_BREWERY = @"Begin Transaction
                                                    UPDATE users SET user_role = 'brewer' WHERE user_id = @userId;
                                                    INSERT INTO breweries(brewery_name, user_id, history, phone, street_address, city, zip_code, is_active) 
                                                    VALUES(@breweryName, @userId, @history, @phone, @streetAddress, @city, @zipCode, @isActive);
                                                    DECLARE @breweryId int = @@IDENTITY;
                                                    SELECT * FROM breweries WHERE brewery_id = @breweryId;
                                                    INSERT INTO brewery_images (brewery_id, image_url) VALUES (@breweryId, @imageUrl);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 1, 0, 0, 'AM', 0, 0, 'AM', 1);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 2, 0, 0, 'AM', 0, 0, 'AM', 1);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 3, 0, 0, 'AM', 0, 0, 'AM', 1);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 4, 0, 0, 'AM', 0, 0, 'AM',1);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 5, 0, 0, 'AM', 0, 0, 'AM', 1);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 6, 0, 0, 'AM', 0, 0, 'AM', 1);
                                                    INSERT INTO hours (brewery_id, day_of_week, open_hour, open_minute, open_am_pm, close_hour, close_minute, close_am_pm, is_closed)
                                                        VALUES (@breweryId, 7, 0, 0, 'AM', 0, 0, 'AM', 1);
                                                    Commit Transaction";
        private const string SQL_UPDATE_BREWERY = @"UPDATE breweries SET user_id = @userId, brewery_name = @breweryName, history = @history, street_address = @streetAddress,
                                                    phone = @phone, city = @city, zip_code = @zipCode, is_active = @isActive WHERE brewery_id = @breweryId;
                                                    SELECT * FROM breweries WHERE brewery_id = @breweryId;";

        public BrewerySqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Brewery> GetBreweries()
        {
            List<Brewery> breweries = new List<Brewery>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_BREWERIES, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Brewery createdBrewery = RowToObject(reader);
                        createdBrewery.DefaultImageUrl = Convert.ToString(reader["image_url"]);
                        breweries.Add(createdBrewery);
                    }

                    return breweries;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        public List<Brewery> GetBreweries(int userId)
        {
            List<Brewery> breweries = new List<Brewery>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_BREWERIES_AUTHENTICATED, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Brewery createdBrewery = RowToObject(reader);
                        createdBrewery.DefaultImageUrl = Convert.ToString(reader["image_url"]);
                        if (DBNull.Value.Equals(reader["favorite"]))
                        {
                            createdBrewery.IsFavorite = false;
                        }
                        else if (Convert.ToInt32(reader["favorite"]) == userId)
                        {
                            createdBrewery.IsFavorite = true;
                        }
                        else
                        {
                            createdBrewery.IsFavorite = false;
                        }

                        breweries.Add(createdBrewery);
                    }

                    return breweries;

                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        public Brewery GetBrewery(int breweryId)
        {
            Brewery brewery = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_BREWERY, conn);
                    cmd.Parameters.AddWithValue("@breweryId", breweryId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        brewery = RowToObject(reader);
                    }

                    return brewery;
                    
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        public Brewery GetBrewery(int breweryId, int userId)
        {
            Brewery brewery = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_BREWERY_AUTHENTICATED, conn);
                    cmd.Parameters.AddWithValue("@breweryId", breweryId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        brewery = RowToObject(reader);

                        reader.NextResult();

                        if (reader.Read())
                        {
                            brewery.IsFavorite = true;
                        }
                        else
                        {
                            brewery.IsFavorite = false;
                        }
                    }

                    return brewery;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        public List<Brewery> GetBreweriesByBrewer(int userId)
        {
            List<Brewery> breweries = new List<Brewery>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_BREWERIES_BY_BREWER, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Brewery brewery = RowToObject(reader);
                        brewery.DefaultImageUrl = Convert.ToString(reader["image_url"]);

                        breweries.Add(brewery);
                    }
                }

                return breweries;
            }
            catch (SqlException ex)
            {

                throw;
            }
        }


        public Brewery CreateBrewery(Brewery brewery)
        {
            Brewery createdBrewery = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(SQL_CREATE_BREWERY, conn);
                    cmd.Parameters.AddWithValue("@breweryName", brewery.BreweryName);
                    cmd.Parameters.AddWithValue("@userId", brewery.UserId);
                    cmd.Parameters.AddWithValue("@history", "");
                    cmd.Parameters.AddWithValue("@streetAddress", "");
                    cmd.Parameters.AddWithValue("@phone", "");
                    cmd.Parameters.AddWithValue("@city", "");
                    cmd.Parameters.AddWithValue("@zipCode", "");
                    cmd.Parameters.AddWithValue("@isActive", true);
                    cmd.Parameters.AddWithValue("@imageUrl", "https://res.cloudinary.com/breweryfinderte/image/upload/v1618520520/bfimages/lrvfz5up7svx4uwgh8wk.jpg");

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        createdBrewery = RowToObject(reader);
                    }
                }
                return createdBrewery;
            }
            catch (SqlException ex)
            {
                throw;
            }
        }

        public Brewery UpdateBrewery(Brewery brewery)
        {
            Brewery updatedBrewery = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_UPDATE_BREWERY, conn);
                    cmd.Parameters.AddWithValue("@userId", brewery.UserId);
                    cmd.Parameters.AddWithValue("@breweryName", brewery.BreweryName);
                    cmd.Parameters.AddWithValue("@history", brewery.History);
                    cmd.Parameters.AddWithValue("@streetAddress", brewery.StreetAddress);
                    cmd.Parameters.AddWithValue("@phone", brewery.Phone);
                    cmd.Parameters.AddWithValue("@city", brewery.City);
                    cmd.Parameters.AddWithValue("@zipCode", brewery.ZipCode);
                    cmd.Parameters.AddWithValue("@isActive", brewery.IsActive);
                    cmd.Parameters.AddWithValue("@breweryId", brewery.BreweryId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        updatedBrewery = RowToObject(reader);
                    }

                }

                return updatedBrewery;
            }
            catch (SqlException ex)
            {

                throw;
            }
        }
        private static Brewery RowToObject(SqlDataReader reader)
        {
            Brewery brewery = new Brewery();

            brewery.BreweryName = Convert.ToString(reader["brewery_name"]);
            brewery.UserId = Convert.ToInt32(reader["user_id"]);
            brewery.BreweryId = Convert.ToInt32(reader["brewery_id"]);
            brewery.History = Convert.ToString(reader["history"]);
            brewery.IsActive = Convert.ToBoolean(reader["is_active"]);
            brewery.StreetAddress = Convert.ToString(reader["street_address"]);
            brewery.Phone = Convert.ToString(reader["phone"]);
            brewery.City = Convert.ToString(reader["city"]);
            brewery.ZipCode = Convert.ToString(reader["zip_code"]);

            return brewery;
        }

    }
}
