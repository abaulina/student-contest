import React from 'react';
import PrivateRoute from './privateRoute';

function PrivatePage() {
	return (
		<PrivateRoute>
			<div className='d-flex user-account'>
				<p className='not-found'>Some info</p>
			</div>
		</PrivateRoute>
	);
}

export default PrivatePage;
