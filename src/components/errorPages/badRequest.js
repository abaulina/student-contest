import React from 'react';
import './badRequest.css';
const badRequest = () => {
	return (
		<div className='d-flex align-items-center flex-column'>
			<p className='badRequest'>
				Something's wrong... You might want to try one more time
				<span aria-label='frowning face with open mouth'>😦</span>
			</p>
			<a role='button' href='/' className='btn btn-outline-primary'>
				Take me back to the homepage
			</a>
		</div>
	);
};

export default badRequest;
