/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ReadBookChapters } from '../models/ReadBookChapters';
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
    public static getUserReadChapters(
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

}
