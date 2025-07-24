/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { ReadBookChapters } from './ReadBookChapters';

export type BibleProgressStats = {
    readTimes?: number;
    currentPercentage?: number;
    readBookChapters?: Array<ReadBookChapters>;
};

