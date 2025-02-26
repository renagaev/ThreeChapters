/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { StructureTestament } from '../models/StructureTestament';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class BibleService {

    /**
     * @returns StructureTestament Success
     * @throws ApiError
     */
    public static getStructure(): CancelablePromise<Array<StructureTestament>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/bible',
        });
    }

}
