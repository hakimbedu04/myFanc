using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFanc.Api.Common;
using MyFanc.BLL;
using MyFanc.BusinessObjects;
using MyFanc.DTO.Internal.Wizard;
using MyFanc.DTO.Internal.Wizards;
using MyFanc.Services;

namespace MyFanc.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WizardController : ControllerBase
    {
        private readonly IBll _bll;

        public WizardController(IBll bll)
        {
            _bll = bll;
        }


        [HttpGet("{wizardId}")]
        public async Task<IActionResult> GetWizardDetail(int wizardId)
        {
            try
            {
                if(wizardId <= 0)
                {
                    return BadRequest();
                }
                var wizard = await _bll.GetWizardDetailAsync(wizardId);
                return wizard == null ? NotFound() : Ok(wizard);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPut("{wizardId}")]
        public async Task<IActionResult> EditWizard(int wizardId, WizardDTO wizardDto)
        {
            try
            {
                if (wizardId <=0)
                {
                    return BadRequest();
                }

                await _bll.EditWizardAsync(wizardDto);
             
                return Ok(wizardId);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }



        [HttpGet("{wizardId}/Questions")]
        public async Task<IActionResult> GetQuestions(int wizardId, [FromQuery] bool isFirstQuestion = false)
        {
            try
            {
                if (wizardId >= 0)
                {
                    var question = await _bll.ListQuestionAsync(wizardId,isFirstQuestion);
                    return Ok(question);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{wizardId}/Questions/{questionId}")]
        public async Task<IActionResult> GetQuestion(int wizardId, int questionId)
        {
            string errors;
            try
            {
                if (wizardId <= 0 && questionId <= 0)
                {
                    errors = Constants.InputMadantoryValues;
                    return BadRequest(errors);
                }
                else
                {
                    var result = await _bll.GetQuestionAsync(wizardId, questionId);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost("{wizardId}/Questions")]
        public async Task<IActionResult> CreateQuestion(int wizardId, CreateQuestionDTO questionDTO)
        {
            string errors;
            try
            {
                if (wizardId >= 0)
                {
                    if (questionDTO == null)
                    {
                        errors = Constants.CreateQuestion;
                        return BadRequest(errors);
                    }
                    var result = await _bll.CreateQuestionAsync(wizardId, questionDTO);
                    return Ok(result);
                }
                else
                {
                    errors = Constants.InputMadantoryValues;
                    return BadRequest(errors);
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPut("{wizardId}/Questions/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int wizardId, int questionId, UpdateQuestionDTO updateQuestion)
        {
            string errors;
            try
            {
                if (wizardId > 0 && questionId > 0)
                {
                    if (updateQuestion == null)
                    {
                        errors = Constants.CreateQuestion;
                        return BadRequest(errors);
                    }
                    var result = await _bll.UpdateQuestionAsync(wizardId, questionId, updateQuestion);
                    return Ok(result);

                }
                else
                {
                    errors = Constants.InputMadantoryValues;
                    return BadRequest(errors);
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpDelete("{wizardId}/Questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int wizardId, int questionId)
        {
            try
            {
                if (wizardId > 0 && questionId > 0)
                {
                    await _bll.DeleteQuestionAsync(wizardId, questionId);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }



        [HttpGet("{wizardId}/Questions/{questionId}/Answers")]
        public IActionResult GetAnswers(int wizardId, int questionId)
        {
            try
            {
                if(wizardId > 0 && questionId > 0)
                {
                    var answers = _bll.ListAnswers(wizardId, questionId);
                    return Ok(answers);
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{wizardId}/Questions/{questionId}/Answers")]
        public async Task<IActionResult> CreateAnswer(int wizardId, int questionId, CreateAnswerDTO answerDto)
        {
            try
            {
                if (wizardId <= 0)
                {
                    return BadRequest();
                }

                var result = await _bll.CreateAnswerAsync(wizardId, questionId, answerDto);
                return Ok(result);
            }
            catch(Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
            
        }

        [HttpGet("{wizardId}/Questions/{questionId}/Answers/{answerId}")]
        public async Task<IActionResult> GetAnswer(int wizardId, int questionId, int answerId)
        {
            try
            {
                if(wizardId > 0 && questionId > 0 && answerId > 0)
                {
                    var result =  await _bll.GetAnswerAsync(wizardId, questionId, answerId);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPut("{wizardId}/Questions/{questionId}/Answers/{answerId}")]
        public async Task<IActionResult> UpdateAnswer(int wizardId, int questionId, int answerId, UpdateAnswerDTO updateAnswerDTO)
        {
            try
            {
                if(wizardId > 0 && questionId > 0 && answerId > 0 && updateAnswerDTO != null)
                {
                    var result = await _bll.UpdateAnswerAsync(wizardId, questionId, answerId, updateAnswerDTO);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{wizardId}/Questions/{questionId}/Breadcrumb")]
        public async Task<IActionResult> GetBreadcrumb(int wizardId, int questionId)
        {
            try
            {
                if(wizardId > 0 && questionId > 0)
                {
                    var breadcrumb = await _bll.GetBreadcrumbAsync(wizardId, questionId);
                    return Ok(breadcrumb);
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPatch("{wizardId}/Questions/{questionId}/Answers/Reorder")]
        public async Task<IActionResult> ReorderAnswer(int wizardId, int questionId, List<AnswerOrderItemDTO> answerOrderItems)
        {
            try
            {
                var result = await _bll.ReorderAnswersAsync(wizardId, questionId, answerOrderItems);
                return NoContent();
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpDelete("{wizardId}/Questions/{questionId}/Answers/{answerId}")]
        public async Task<IActionResult> DeleteAnswer(int wizardId, int questionId, int answerId)
        {
            try
            {
                if (wizardId > 0 && questionId > 0 && answerId > 0)
                {
                    await _bll.DeleteAnswerAsync(wizardId, questionId, answerId);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }
    }
}
