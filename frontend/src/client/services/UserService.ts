/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BibleProgressStats } from '../models/BibleProgressStats';
import type { DayChaptersReadDto } from '../models/DayChaptersReadDto';
import type { UserDetails } from '../models/UserDetails';
import type { UserDto } from '../models/UserDto';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class UserService {

    /**
     * @returns UserDto Success
     * @throws ApiError
     */
    public static getUsers(): CancelablePromise<Array<UserDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users',
        });
    }

    /**
     * @param userId
     * @returns BibleProgressStats Success
     * @throws ApiError
     */
    public static getUserBibleProgress(
        userId: number,
    ): CancelablePromise<BibleProgressStats> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users/{userId}/bible-progress',
            path: {
                'userId': userId,
            },
        });
    }

    /**
     * @param userId
     * @returns DayChaptersReadDto Success
     * @throws ApiError
     */
    public static getUserReadChaptersByDay(
        userId: number,
    ): CancelablePromise<Array<DayChaptersReadDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users/{userId}/read-chapters-by-day',
            path: {
                'userId': userId,
            },
        });
    }

    /**
     * @param userId
     * @returns UserDetails Success
     * @throws ApiError
     */
    public static getUserDetails(
        userId: number,
    ): CancelablePromise<UserDetails> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users/{userId}',
            path: {
                'userId': userId,
            },
        });
    }

    /**
     * @param telegramId
     * @returns number Success
     * @throws ApiError
     */
    public static getUserIdByTelegramId(
        telegramId?: number,
    ): CancelablePromise<number> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users/by-telegram-id',
            query: {
                'telegramId': telegramId,
            },
        });
    }

    /**
     * @param userId
     * @param fileName
     * @returns any Success
     * @throws ApiError
     */
    public static getUserAvatar(
        userId: number,
        fileName: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users/{userId}/avatar/{fileName}',
            path: {
                'userId': userId,
                'fileName': fileName,
            },
        });
    }

}
