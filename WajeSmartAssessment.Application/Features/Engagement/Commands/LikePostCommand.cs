﻿using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Engagement.Commands;
public record LikePostCommand(string PostId) : IRequest<ApiResponse>;

