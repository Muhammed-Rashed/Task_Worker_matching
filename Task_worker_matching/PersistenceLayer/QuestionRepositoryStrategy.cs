using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Task_worker_matching.Memory_Layer;

namespace PersistenceLayer
{
    public class QuestionRepositoryStrategy : IRepositoryStrategy<Question>
    {
        private PersistenceManager PM = PersistenceManager.GetInstance();

        public bool addAnswers(List<Answer> answers, int question_id, SqlConnection conn)
        {
            try
            {
                foreach (var ans in answers)
                {
                    string insertQuery = @"
                    INSERT INTO Anwer (Question_id, Answer_text, Answerer_id, Answerer_type, Answer_time, Answer_rate)
                    VALUES (@Question_id, @Answer_text, @Answerer_id, @Answerer_type, @Answer_time, @Answer_rate);";

                    using var cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Question_id", question_id);
                    cmd.Parameters.AddWithValue("@Answer_text", ans.get_answer());
                    cmd.Parameters.AddWithValue("@Answerer_id", ans.get_answerer().get_user_ID());

                    // Check the user type
                    if (ans.get_answerer() is Client)
                    {
                        cmd.Parameters.AddWithValue("@Answerer_type", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Answerer_type", 1);
                    }

                    cmd.Parameters.AddWithValue("@Answer_time", ans.get_answer_time());
                    cmd.Parameters.AddWithValue("@Answer_rate", ans.get_answer_rate());

                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding Answers: {ex.Message}");
                return false;
            }
        }

        public bool add_answer(Answer answer, int question_id)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string insertQuery = @"
                    INSERT INTO Anwer (Question_id, Answer_text, Answerer_id, Answerer_type, Answer_time, Answer_rate)
                    VALUES (@Question_id, @Answer_text, @Answerer_id, @Answerer_type, @Answer_time, @Answer_rate);";

                    using var cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Question_id", question_id);
                    cmd.Parameters.AddWithValue("@Answer_text", answer.get_answer());
                    cmd.Parameters.AddWithValue("@Answerer_id", answer.get_answerer().get_user_ID());

                    // Check the user type
                    if (answer.get_answerer() is Client)
                    {
                        cmd.Parameters.AddWithValue("@Answerer_type", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Answerer_type", 1);
                    }

                    cmd.Parameters.AddWithValue("@Answer_time", answer.get_answer_time());
                    cmd.Parameters.AddWithValue("@Answer_rate", answer.get_answer_rate());
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding Answers: {ex.Message}");
                return false;
            }
        }
        public bool add_item(Question item)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string insertQuery = @"
                        INSERT INTO Question (Asker_id, Asker_type, Question_text, Question_time, Question_rate)
                        OUTPUT INSERTED.Id
                        VALUES (@Asker_id, @Asker_type, @Question_text, @Question_time, @Question_rate);";

                    using var cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Asker_id", item.get_asker().get_user_ID());

                    // Check the user type
                    if (item.get_asker() is Client)
                    {
                        cmd.Parameters.AddWithValue("@Asker_type", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Asker_type", 1);
                    }

                    cmd.Parameters.AddWithValue("@Question_text", item.get_question());
                    cmd.Parameters.AddWithValue("@Question_time", item.get_question_time());
                    cmd.Parameters.AddWithValue("@Question_rate", item.get_question_rate());

                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    item.set_id(newId); // Set the generated ID


                    return addAnswers(item.get_answers(), item.get_id(), conn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding Question: {ex.Message}");
                return false;
            }
        }

        public bool delete_item(Question item)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string deleteQuery = @"
                        DELETE FROM Question
                        WHERE Id = @Id";

                    using var cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", item.get_id());

                    int rowsAffected = cmd.ExecuteNonQuery();

                    string deleteAnswersQuery = @"
                        DELETE FROM Answer
                        WHERE Question_id = @QId";

                    using var cmdANS = new SqlCommand(deleteAnswersQuery, conn);
                    cmdANS.Parameters.AddWithValue("QId", item.get_id());

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting question: {ex.Message}");
                return false;
            }
        }

        public List<Answer> get_answers(int question_id, SqlConnection conn)
        {
            var answers = new List<Answer>();
            try
            {
                string selectQuery = @"
                    SELECT Answer_text, Answerer_id, Answerer_type, Answer_time, Answer_rate
                    FROM Answer
                    WHERE Question_id = @QId";

                using var cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@QId", question_id);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var answer = new Answer();
                    answer.set_answer(reader.GetString(0));

                    int userType = reader.GetInt16(2);
                    User answerer;
                    if (userType == 0)
                        answerer = new Client();
                    else
                        answerer = new Worker();

                    answerer.set_user_ID(reader.GetInt32(1));
                    answer.set_answerer(answerer);

                    answer.set_answer_time(reader.GetDateTime(3));
                    answer.set_answer_rate(reader.GetDouble(4));

                    answers.Add(answer);
                }
                return answers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker offers: {ex.Message}");
                return answers;
            }
        }

        public List<Question> get_items(int user_id)
        {
            var questions = new List<Question>();
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string selectQuery = @"
                        SELECT Id, Asker_id, Asker_type, Question_text, Question_time, Question_rate
                        FROM Question";

                    using var cmd = new SqlCommand(selectQuery, conn);

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var question = new Question();
                        question.set_id(reader.GetInt32(0));

                        // Add a temp user that will be replaced in the service layer
                        int userType = reader.GetInt16(2);
                        User asker;
                        if (userType == 0)
                            asker = new Client();
                        else
                            asker = new Worker();

                        asker.set_user_ID(reader.GetInt32(1));
                        question.set_asker(asker);
                        question.set_question(reader.GetString(3));
                        question.set_question_time(reader.GetDateTime(4));
                        question.set_question_rate(reader.GetDouble(5));
                        question.set_answers(get_answers(question.get_id(), conn));

                        questions.Add(question);
                    }
                }
                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving questions: {ex.Message}");
                return questions;
            }
        }

        public bool update_item(Question new_item, Question old_item)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string updateQuery = @"
                        UPDATE Question
                        SET Question_text = @Question_text,
                            Question_time = @Question_time,
                            Question_rate = @Question_rate
                        WHERE Id = @Id AND Asker_id = @Asker_id";

                    using var cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Question_text", new_item.get_question());
                    cmd.Parameters.AddWithValue("@Question_time", new_item.get_question_time());
                    cmd.Parameters.AddWithValue("@Question_rate", new_item.get_question_rate());
                    cmd.Parameters.AddWithValue("@Id", old_item.get_id());
                    cmd.Parameters.AddWithValue("@Asker_id", old_item.get_asker().get_user_ID());

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating worker offer: {ex.Message}");
                return false;
            }
        }

        public bool update_answer(Answer new_answer, Answer old_answer, int question_id)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string updateQuery = @"
                        UPDATE Question
                        SET Answer_text = @Answer_text,
                            Answer_time = @New_Answer_time,
                            Answer_rate = @Answer_rate
                        WHERE Question_id = @Question_id AND Answerer_id = @Answerer_id AND Answer_time = @Old_Answer_time";

                    using var cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Answer_text", new_answer.get_answer());
                    cmd.Parameters.AddWithValue("@New_Answer_time", new_answer.get_answer_time());
                    cmd.Parameters.AddWithValue("@Answer_rate", new_answer.get_answer_rate());
                    cmd.Parameters.AddWithValue("@Question_id", question_id);
                    cmd.Parameters.AddWithValue("@Answerer_id", old_answer.get_answerer().get_user_ID());
                    cmd.Parameters.AddWithValue("@Answer_time", old_answer.get_answer_time());

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating worker offer: {ex.Message}");
                return false;
            }
        }
    }
}