
// �� ����  ������� �����!
void thread_function_official(uint64_t  leftBorder, uint64_t  rightBorder, uint64_t  maxCount, uint64_t  * simples, mutex *locker, FILE*file, std::vector<uint128_t> *spps )
{
	uint128_t q;
	uint128_t n;

	uint128_t u;
	uint128_t ords[A_LENGTH];

	int j;
	int koef;

	cpp_int d;
	cpp_int sqrt_d;

	uint128_t k = 1;
	cpp_int d1, d2;

	for (uint64_t i = leftBorder; i < rightBorder && i < maxCount; i++ ) {

		// ������� ord �� ������ ����
		for ( j = 0; j < A_LENGTH; j++ ) {
			ords[j] = getOrd( simples[ i ], A_uint128_t[ j ] );
		}
		ords[j] = 0;

		// ������� ��� �� ���� ord
		u = getNOK(ords);

		if ( u % 2 != 0 ) {

			koef = 2;
		}
		else {

			koef = 1;
		}

		// ���������������� ������� ��������
		thread t1([ &d1, simples, i ] {
			d1 = pow((cpp_int)2, simples[ i ] - 1) - 1;
		});
		thread t2([ &d2, simples, i ] {
			d2 = pow((cpp_int)3, simples[ i ] - 1) - 1;
		});

		t1.join();
		t2.join();

		d = gcd( d1, d2 );

		sqrt_d = sqrt( d ) + 1;

		for (k = 1, q = 1; q < sqrt_d ; k++) {

			q = koef * k * u + 1;

			if ( d % q == 0 ) {

				n = q * simples[ i ];

				if ( LABS_TEST_MILLER_RABIN_uint128_t( &n, 2 ) ) {

					locker->lock();
					if ( std::find(spps->begin(), spps->end(), n) == spps->end()) {
						spps->push_back(n);
						printValues(&n, simples[i], &q, file);
					}
					locker->unlock();
				}
				else {
					// pass
				}
			}
			else {
				// pass
			}
		}

	}

	// ������ ��������� ��� �������
}

// �������, ������� ��������� ���  ����� � ���������� �� start �� finish � ������� ������� threadFunctionRun3 
void official_algorithm_run() {

	// ������������ ���������� ������� �����. �� ��������� ����� ������� ������� ���������������� ����������
	uint64_t  max_count_simples = 1000000;
	uint64_t  *simples = new uint64_t[ max_count_simples ];

	// ������� ���������� ������� ����� � ��� ������� �����
	uint64_t  count_simples = 0;// getCountSimples(3, max_count_simples, simples);
	getPrimes( simples, &count_simples, 0, max_count_simples, 1 );

	uint64_t  step = 200;// count_simples / 25;

	std::atomic<bool> * is_completed_threads = NULL;
	FILE *f = NULL;

	ThreadsManager 	example;
	fprintf(example.FOUT_FILE, "����������: �� %lld, ������� ����� ����� : %lld, ������������ ����� = %lld\n\n", max_count_simples, count_simples, simples[count_simples - 2]);
	printf("����������: �� %lld, ������� ����� ����� : %lld, ������������ ����� = %lld\n\n", max_count_simples, count_simples, simples[count_simples - 2]);

	// ���������!
	example.parallel_by_cores(is_completed_threads, f, count_simples, simples, step, 100, thread_function_official, 2);
}
