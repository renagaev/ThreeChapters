/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { DayChaptersReadDto } from '../models/DayChaptersReadDto';
import type { ReadBookChapters } from '../models/ReadBookChapters';
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
     * @returns ReadBookChapters Success
     * @throws ApiError
     */
    public static getUserReadChaptersByBook(
        userId: number,
    ): CancelablePromise<Array<ReadBookChapters>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/users/{userId}/read-chapters',
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

}
